using System;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;

namespace WackAMole {

    public struct Tile {// I made a 'Tile' struct because I needed a way to asscociate the button with the indication of it being used by another thread whithout having a thread wait on a lock
        public Tile(Button t, bool m) {
            Btn = t;
            Used = m;
        }
        public Button Btn { get; set; }
        public bool Used { get; set; }
    }

    public partial class Form1 : Form {

        /// <summary>
        /// PERSONAL SUGGESTIONS:
        /// Perhaps have a difficulty, a.k.a. change the up time and/or amount of tiles option
        /// Make a cooler start up
        /// Have all of the empty tiles default as the Hole png
        /// Have the threads remember the last coordinate they had so they don't choose the same tile after a cycle
        /// Move the pause check in the animation as well, and remove the stack so the moles will stop in place, like a real pause in a video game
        /// </summary>

        private Tile[,] grid;
        private Thread killCheck;//Used for checking active threads while going through the process of ending the game
        private Random r = new Random();//used when loading the stack
        private Thread[] thrds = new Thread[5];
        private readonly Mole imgs = new Mole();//Image management class
        private Stack<int> wait = new Stack<int>();//the stack will contain different numbers for the moles' emergance to become spaced out 
        private EventWaitHandle pauseResume = new ManualResetEvent(true);//Pause/Resume

        private object handler = new object();//Key
        private bool paused = false, abort = false;//abort is for the end condition; con is for the selection on the tiles won't be counted as anythin while it's paused
        private int score = 0, miss = 0, mirror = 0;//self explanitory; mirror is the value the threads will have to mimic to know when it has already woken up from the pause operation

        private delegate void zpd();//Zero parameter delegate; used for 'Stopping...' / 'Game Ended' bottom text
        private delegate void sgd(object o, EventArgs e);//Stop Game Delegate
        private delegate void cyc(int x, int y, Mole m, int i); //delegate used for cycling the animation
        private delegate void strUpdate(Label obj, string str);//delegate for updating 

        public Form1() {
            InitializeComponent();
        }//Form

        //Tasks are not being used because these operations are in an infinate loop; threads are being used for too long to take advantage of threadpooling

        //The only lock is for the stack.
        //Locks are not being used (as much) because if a thread attepts to use a tile that has a lock it'll have to wait on it, and that's not what I want.
        //I want the thread to try a different tile that's not being used if the one it attempts to use is already in use and not have to wait on that same tile.
        //So the solution is to check if the tile is being used using the 'Used' boolean, and if it is then skip code and try again for a different tile; i.e. make sure they won't use the same tile.
        //I'm also not using a lock for the Tile array although multiple threads are accessing it at the same time, because I WANT multiple threads to be access it in parallel. Access confilicts are resolved through the 'Used' boolean.
        //Reasoning: I want the moles to pop out while another mole is active and for that to happen the threads will have to access the array and choose different tiles while
        //other threads are accessing the array, so I can't have multiple threads waiting to use the array, otherwise the moles would come out one at a time.

        private void Action(object sender, EventArgs e) {
            if (paused || abort) return; //paused is to prevent misses and scores while paused; abort: if game is over then just return, no need to run this code anymore if the game is over
            Button btn = (Button)sender;
            if (btn.BackgroundImage != null && !(btn.BackgroundImage==imgs.animation[imgs.animation.Length-1])) {//If the tile has an image on it; the only time this could be true is when there is a mole on the tile,
                score += 100;                                                                                   //since that's the only time an image will be on the tile. Also check if Pow is on the tile to prevent cheating points;
                lblScore.Text = score.ToString();                                                              //since Pow is an image and would make this not null, this would result as a percieved point if hit again.
                #region Win Condition
                /*//WIN CONDITION; 50000 is the current win condition; none is required nor specified for this assignment
                if (score >= 50000 && !abort)  {//Win condition, also checks for the abort boolean to prevent multiple MessageBoxes from other threads after the game is over
                    abort = true;//signal that the game is over
                    MessageBox.Show(String.Format($"Misses: {miss}\nScore: {score}\nWinner! Game Over"), "WINNER");//Exit
                    BtnStopGame_Click(null, null);
                }
                */
                #endregion
                btn.BackgroundImage = imgs.animation[imgs.animation.Length - 1];//assign the image to the Pow image object from mole class, so when the thread compares the Pow image it compares the image object's address
            } else {//Miss condition                                         
                miss++;
                if (lblMisses.InvokeRequired)//idle Miss; This invocation happens once the thread is done with the animation (which will only happen if it's not interrupted by the user)
                    lblMisses.Invoke(new strUpdate(StrUpd), new object[] { lblMisses, miss.ToString() });//Update the miss label
                else  //if the user hit's an incorrect tile
                    lblMisses.Text = miss.ToString();//update string
                //LOSE CONDITION; 20 is the current lose condition
                if (miss >= 20 && !abort) {//Lose condition, checks if the game is already over to prevent multiple MessageBoxes from appearing from idle misses. Might be true, might not; this comment is a relic from a previous build.
                    abort = true;//Notify threads of game over condition
                    MessageBox.Show(String.Format($"Misses: {miss}\nScore: {score}\nYou Lose, Game Over"), "LOSS");
                    if (lblMisses.InvokeRequired) //If it's due to an idle miss loss
                        lblMisses.Invoke(new sgd(BtnStopGame_Click), new object[] { null, null });//invoke stop game method
                    else //if it's not an idle miss loss; i.e. hitting the wrong tile
                        BtnStopGame_Click(null, null);
                }
            }//win-lose if-else chain
        }//Action

        private void StrUpd(Label lbl, string str) {
            lbl.Text = str;
        }//String update delegate method

        private void BtnSetGrid_Click(object sender, EventArgs e) {
            lblStoppingState.Visible = false;//Set the bottom text to false; left visible from a previous game
            BtnSetGrid.Enabled = false;//Disable the Set Grid button
            abort = false;//both abort and pause could be left as true from a previous play
            paused = false;//CHECK IF THIS LINE NEEDS TO BE HERE
            BtnStopGame.Enabled = true;
            BtnPauseResume.Enabled = true;
            SetGrid();
            SpawnThreads();
        }//Start Button
        
        private void SetGrid() {
            if (grid != null) return; //If the grid has alrady been created then just return and use the previously created grid
            int left = 100, top = 100;
            grid = new Tile[5, 5];//This should still work dynamically
            for (int i = 0; i < 5; i++) {//Row
                for (int j = 0; j < 5; j++) {//Column
                    Tile t = new Tile(new Button(), false);//Instance a button and assign the 'Used' as false
                    grid[i, j] = t;
                    t.Btn.Top = top;
                    t.Btn.Left = left;
                    t.Btn.Height = 100;
                    t.Btn.Width = 100;
                    t.Btn.Click += Action;
                    t.Btn.BackgroundImageLayout = ImageLayout.Stretch;
                    t.Btn.BackgroundImage = null;
                    Controls.Add(t.Btn);
                    left += 100;
                    Update();
                    Thread.Sleep(10);//Yes, I made this sleep because I thought the user seeing the grid being built looked cool
                }
                top += 100;//move a row down and set position back to the left
                left = 100;
            }
        }

        private void SpawnThreads() {
            Thread spawn = new Thread(() => {//Spawning threads
                Stopwatch sw = new Stopwatch();//This will keep track of when the thread should spawn the next thread
                thrds[0] = new Thread(() => { Cycle(); });//The first thread is hard coded because I want it to spawn immediatly
                thrds[0].IsBackground = true;//All threads are going to be background threads so there's no waiting/errors from the threads if the program is abruptly ended by the user
                thrds[0].Start();
                //Console.WriteLine("Thread 0 has been instanced");
                Random r = new Random();
                int tick = r.Next(10000, 14001);
                sw.Start();
                for(int i = 1; i < thrds.Length;)  {
                    if (paused)//if the game is paused then keep restarting until the game is unpaused then resume spawning
                        sw.Restart();        
                    if (abort) break;//if the game has ended before all threads are spawned then exit
                    if(sw.ElapsedMilliseconds >= tick) {//if it's time to spawn another thread 
                        thrds[i] = new Thread(() => { Cycle(); });
                        thrds[i].IsBackground = true;
                        thrds[i].Start();
                        //Console.WriteLine($"Thread {i} has been instanced");
                        i++;
                        tick = r.Next(10000, 14001);//wait for another 10000-14000ms
                        sw.Restart();
                    }
                }
                //Console.WriteLine("All thread spawnings are completed.");
            });
            spawn.IsBackground = true;//So the program ends whenever the user just exits without it finishing it's thread spawning
            spawn.Start();
        }

        private void Cycle() {
            Random rCoord = new Random();//random is used to choose a random tile
            int x, y, sleepTime, reflection=mirror, index=0;//Index is for the animation, reflection is equal to mirror to keep the threads in sync with the abount of times the game has been paused since they spawn at different times
            Stopwatch sw = new Stopwatch();//used for 'waking up' the threads after the game is unpaused, and check for abort condition while waiting for it's wake up time. 
            while (true) {//loop          //I'm not using Thread.Sleep(x); becuase the player would have to wait upwards of 40 seconds for the game to end and that's unacceptable for me
                pauseResume.WaitOne();//gate
                if (abort) break;//check for abort here because the StopGame button also opens the gate after making abort = true; so if the game is ended while the game's paused there's no need to wait for a whole cycle, just end.
                if (reflection != mirror) {//If reflection is not == to mirror then the game has been paused before; The thread sees this after it unpauses, so now it'll have to sleep for 10000-14000ms sequentially to 4 threads
                    if (wait.Count == 0) {//If the stack is empty. There shouldn't be a time where this happens, but is here to prevent a possible error. If there's nothing in the stack then the thread should be awake.
                    } else {
                        reflection = mirror;//set to mirror so let the thread know in other iteraitons that it has done it's unpause code already
                        lock (handler)//Lock is here so there's no shinanigans with stack access; first come first serve, but not allowed to have the same wait time.
                            sleepTime = wait.Pop();
                        //Console.WriteLine($"Thread going to sleep for {sleepTime}ms");
                        sw.Start();
                        while (true) {//wait for a time determined by the stack. 
                            if (abort) break;//if the game ends while waiting then exit; to prevent waiting a time upwards of 40s in the 
                            if (sw.ElapsedMilliseconds >= sleepTime) break;//Wake up
                        }
                        //Console.WriteLine($"Thread woke up after {sleepTime}ms");
                        sw.Reset();//Reset stopwatch for next time use
                    }
                    if (abort) break;//check if aborted while it was paused
                }
                x = rCoord.Next(0, 5);//x and y coordinate for tiles
                y = rCoord.Next(0, 5);
                if (grid[x, y].Used)//checks if the tile it's chosen is already being used my another thread
                    continue;//if so then continue and try for a different tile
                grid[x, y].Used = true;//if it's not being used then set the tile to being used first before doing any operations on the tile
                cyc del = new cyc(Cycle);//invoke check is not required since it's garunteed to be cross-threading
                for (index = 0; index < imgs.animation.Length - 1; index++) {//using index < .Length-1 instead of index < .Length because the last image is used for the hits (Pow)
                    if (grid[x, y].Btn.BackgroundImage == imgs.animation[imgs.animation.Length - 1])//if the image is the Pow image set by the action method then the mole has been hit then end
                        break;
                    try {
                        grid[x, y].Btn.Invoke(del, new object[] { x, y, imgs, index });//change image
                    } catch (System.ComponentModel.InvalidAsynchronousStateException) { Thread.CurrentThread.Abort(); }//There seems to be an exception that happens when the game has ended and the cycle thread is trying to animate
                    Thread.Sleep(70);//there are 40 images being cycled for this animation, I know it's 70, but it should be 75ms*40 == 3000ms, so it'd be up for 3 seconds, but code execution takes up some time so I shaved off some ms
                }//animation loop
                if (paused || imgs.animation[imgs.animation.Length-1] == grid[x,y].Btn.BackgroundImage){//'paused' here will make sure the mole won't be an idle miss by using the continue here; If img is Pow then the hit is successful
                    Thread.Sleep(25);//Leavin the image up to let the user know that the mole has successfully been hit                                                                           
                    grid[x, y].Btn.BackgroundImage = null;//set the tile image to null to stop the posibility of cheating points
                    grid[x, y].Used = false;//no longer being used
                    if (abort) break;//Check for abort condition
                    if(!paused) Thread.Sleep(1500);//maybe make this sleep for 1500 - 3000 seconds
                    continue;//to stop idle misses being prompted by the rest of the code, it continues
                }
                grid[x, y].Btn.BackgroundImage = null;//set image to null because if it's an idle miss then the image would be the Hole.png image
                if (abort) { //in then case it's supposed to end then make sure the grid is unused before breaking and skipping the idle miss code
                    grid[x, y].Used = false;
                    break;
                }
                Action(grid[x, y].Btn, null);//action method, garuntee a miss since image is null
                grid[x, y].Used = false;//no longer being used
            }//while
            Thread.CurrentThread.Abort();//Kill the thread
        }//cycle
        private void Cycle(int x, int y, Mole obj, int index) {
            grid[x, y].Btn.BackgroundImage = obj.animation[index];//change and update the image
            Update();
        }//Cycle the animation

        private void BtnPauseResume_Click(object sender, EventArgs e) {
            paused = !paused;//invert the paused boolean
            if (paused) { //If the user chose to pause
                lblStoppingState.Text = "Paused";//Bottom text
                lblStoppingState.Visible = true;//show bottom text
                BtnPauseResume.Text = "Resume";//change button text
                pauseResume.Reset();//Close the gate
                wait.Clear();//clear the stack from previous pauses if all the threads have not been instanced
                //Console.WriteLine("Stack:");
                for (int i = thrds.Length - 1; i >= 0; i--) {
                    wait.Push(r.Next(10000, 14000) * i);
                    //Console.WriteLine(timer.Peek());
                }
                mirror++;//increment mirror to let the threads know it has been paused, and for the threads to check once the gate has been opened
            } else {
                lblStoppingState.Visible = false;//hide bottom text
                BtnPauseResume.Text = "Pause";//change button text
                pauseResume.Set();//close the gate
            }
        }//Pause-Resume

        private void BtnStopGame_Click(object sender, EventArgs e) {
            abort = true; //Let all the threads start the kill proccess
            BtnPauseResume.Enabled = false; //disable the pause button 
            paused = false; //reset the paused value
            mirror = 0;//reset the mirror value
            wait.Clear();//clear the wait stack 
            pauseResume.Set();//open the gate
            BtnPauseResume.Text = "Pause"; //set pause button text to 'Pause'
            score = 0;//reset score
            miss = 0;//reset misses
            lblMisses.Text = "0"; //hardcoded to '0' because it's garunteed to be 0 and I don't want the program
            lblScore.Text = "0"; //to spend more time executing a method
            BtnStopGame.Enabled = false; //Can't request another stop while it's waiting for all the threads to die
            lblStoppingState.Text = "Stopping..."; //Let the user know that the program is in the proccess of stopping the game
            lblStoppingState.Visible = true; //show bottom text
            killCheck = new Thread(() => { //have another thread check if all the threads are dead so the form can still be active
                for (int i = 0; i < 5; i++)
                    if (thrds[i] == null) {//if thread hasn't been spawned then just increment, can't allow an IsAlive check because of null pointer exception
                    } else if (thrds[i].IsAlive) { i--; }//if thread is still alive then decrement so i will be stuck at the same value until the thread is dead. If it is dead then it will allow i to increment
                try { 
                    BtnSetGrid.Invoke(new zpd(GameOverLblUpd));//End game proccess is complete, let the player know and allow them to start again 
                } catch (System.ComponentModel.InvalidAsynchronousStateException) { Thread.CurrentThread.Abort(); }//This exception shouldn't occur due to this being a background thread, but just in case ya'know.
                //Console.WriteLine("\nThread killing proccess is finished\n");
            }); 
            killCheck.IsBackground = true;
            killCheck.Start();
        }//Stop Game

        private void GameOverLblUpd() {
            lblStoppingState.Text = "Game Ended.";
            BtnSetGrid.Enabled = true;
        }

    }//Class

}//Namespace
