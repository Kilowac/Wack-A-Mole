using System;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;

namespace WackAMole {

    public struct Tile {
        public Tile(Button t, bool m) {
            Btn = t;
            Used = m;
        }
        public Button Btn { get; set; }
        public bool Used { get; set; }
    }

    public partial class Form1 : Form {

        private Tile[,] grid;
        private Thread killCheck;
        private Random r = new Random();
        private Thread[] thrds = new Thread[5];
        private readonly Mole imgs = new Mole();
        private Stack<int> wait = new Stack<int>();
        private EventWaitHandle pauseResume = new ManualResetEvent(true);

        private object handler = new object();
        private bool paused = false, abort = false;
        private int score = 0, miss = 0, mirror = 0;

        private delegate void zpd();
        private delegate void sgd(object o, EventArgs e);
        private delegate void cyc(int x, int y, Mole m, int i); 
        private delegate void strUpdate(Label obj, string str);

        public Form1() {
            InitializeComponent();
        }

        private void Action(object sender, EventArgs e) {
            if (paused || abort) return;
            Button btn = (Button)sender;
            if (btn.BackgroundImage != null && !(btn.BackgroundImage==imgs.animation[imgs.animation.Length-1])) {
                score += 100;                                                                                   
                lblScore.Text = score.ToString();                                                              
                #region Win Condition
                /*//WIN CONDITION; 50000 is the current win condition; none is required nor specified for this assignment
                if (score >= 50000 && !abort)  {//Win condition, also checks for the abort boolean to prevent multiple MessageBoxes from other threads after the game is over
                    abort = true;//signal that the game is over
                    MessageBox.Show(String.Format($"Misses: {miss}\nScore: {score}\nWinner! Game Over"), "WINNER");//Exit
                    BtnStopGame_Click(null, null);
                }
                */
                #endregion
                btn.BackgroundImage = imgs.animation[imgs.animation.Length - 1];
            } else {
                miss++;
                if (lblMisses.InvokeRequired)
                    lblMisses.Invoke(new strUpdate(StrUpd), new object[] { lblMisses, miss.ToString() });
                else  
                    lblMisses.Text = miss.ToString();
                
                if (miss >= 20 && !abort) {
                    abort = true;
                    MessageBox.Show(String.Format($"Misses: {miss}\nScore: {score}\nYou Lose, Game Over"), "LOSS");
                    if (lblMisses.InvokeRequired) 
                        lblMisses.Invoke(new sgd(BtnStopGame_Click), new object[] { null, null });
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
            Random rCoord = new Random();
            int x, y, sleepTime, reflection=mirror, index=0;
            Stopwatch sw = new Stopwatch();
            while (true) {
                pauseResume.WaitOne();
                if (abort) break;
                if (reflection != mirror) {
                    if (wait.Count == 0) {
                    } else {
                        reflection = mirror;
                        lock (handler)
                            sleepTime = wait.Pop();
                        
                        sw.Start();
                        while (true) {
                            if (abort) break;
                            if (sw.ElapsedMilliseconds >= sleepTime) break;//Wake up
                        }
                        
                        sw.Reset();
                    }
                    if (abort) break;
                }
                x = rCoord.Next(0, 5);
                y = rCoord.Next(0, 5);
                if (grid[x, y].Used)
                    continue;
                grid[x, y].Used = true;
                cyc del = new cyc(Cycle);
                for (index = 0; index < imgs.animation.Length - 1; index++) {
                    if (grid[x, y].Btn.BackgroundImage == imgs.animation[imgs.animation.Length - 1])
                        break;
                    try {
                        grid[x, y].Btn.Invoke(del, new object[] { x, y, imgs, index });
                    } catch (System.ComponentModel.InvalidAsynchronousStateException) { Thread.CurrentThread.Abort(); }
                    Thread.Sleep(70);
                }
                if (paused || imgs.animation[imgs.animation.Length-1] == grid[x,y].Btn.BackgroundImage){
                    Thread.Sleep(25);
                    grid[x, y].Btn.BackgroundImage = null;
                    grid[x, y].Used = false;
                    if (abort) break;
                    if(!paused) Thread.Sleep(1500);
                    continue;
                }
                grid[x, y].Btn.BackgroundImage = null;
                if (abort) { 
                    grid[x, y].Used = false;
                    break;
                }
                Action(grid[x, y].Btn, null);
                grid[x, y].Used = false;
            }
            Thread.CurrentThread.Abort();
        }
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
