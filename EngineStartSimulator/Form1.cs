using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;




namespace EngineStartSimulator
{
    public partial class mainWindow : Form
    {

        public Gauge[] gauges = new Gauge[7]; // an array of the gauge objects
        Boolean[] onState = new Boolean[7]; // idicates if gauges are on or off, an array of 7 states
        public int direction = 1; // a direction clarifie to tell direction of the gauge movement; 1 is moving upward, -1 is decreasing
        public int memDump = 0; // A counter to keep memory usage maxed around 500 mb.

        // Set up the window! Call the initializer functions and set the N2 gauge ready for movement
        public mainWindow()
        {
            // Initializes window components
            InitializeComponent();
        }

        // Form will load, start the timer that allows the splash and then loads the main screen.
        // Also hide the help panel, since it is actuallty in front
        // And call the center toggle method to put the fuel toggle in the right screen location.
        private void Form1_Load(object sender, EventArgs e)
        {
            // get the splash screen up
            splashImage.BringToFront();

            // Initializes the gauges to their starting locations
            initializeGauges();

            // Makes N2 gauge ready for action
            onState[0] = true;

            // center the splash image, render the main screen (splitContainer1), start with the mennu out
            //centerSplashImage();
            splitContainer1.Show();
            menuOut();
            
            // Timer handles splash screen display time. While window loads up hold the splash image.
            timer1.Start();
            helpPanel.Hide();
            centerToggle(0);
            relocateInfoButton();
            

            // sets all gauges ready to action in starting position
            for (int i = 0; i < gauges.Length; i++)
            {
                gauges[i].move();
            }

        }

        // Declare some necessary variables and State indicators
        int tutorMode = -1; //indicates tutor mode; -1 is off
        int playPause = 1; // indicates play or pause state; play is 1, pause is -1
        int splitterLoc = 0; // Indicates current location of the menu splitter
        int fuelOn = -1; // indicates if the fuel is off or on; -1 is off, 1 is on
        int startButtonOn = -1; // indicates the start button is depressed; -1 is not depressed, 1 is depressed.
        int goTime = 0; // How many times the start button has been pressed
        String mode = "No Error Conditions"; // stores the current selected mode. Default is No Error Condition
        int flood = 0; // indicates if engine has fuel flood. 0 is now flood, 1 is flooded, 2 is flooded early on
        Boolean errorOccured = false; // Notifies if there was a user error
        int errorHandled = 0; // notifies if the user correctly solved the scenario or not. 0 indicates not yet handled, 1 is handled successfully, 0 is failure
        int goFinished = 0; // Lets the program know if a scenario finished or started.
        int firstStopped = 0; // counts the number of times the stopped function goes through
        float lastPos = 0; // This keeps track of the last position of the EGT
        bool disengaged = false; // Makes known the state of engine starter engegement or not
        int attemptOff = 0; // Track the number of attempts to turn off the starter valve when stuck open
        int fuelOnBlocked = -1; // A fuel on tracker for blocked fuel
        bool isRand = false; // ensures the random mode will continue even after changing mode type

        // Set the various affects for mouse actions on in screen buttons
        // first the hamburger menu
        //when clicked, reduce or expand the splitter distance
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            hamburgerBox.BackColor = Color.Transparent;
            if (splitContainer1.SplitterDistance > 45)
            {
                menuIn();
            }
            else if(splitContainer1.SplitterDistance == 45)
            {
                menuOut();
            }
        }

        // Accident 
        private void splitContainer1_MouseHover(object sender, EventArgs e)
        {
            
        }

        // If just mousing over the menu highlight in grey
        private void hamburgerBox_MouseHover(object sender, EventArgs e)
        {
            hamburgerBox.BackColor = Color.DarkGray;
        }

        //if moving the mouse away, go back to original trasparency
        private void hamburgerBox_MouseLeave(object sender, EventArgs e)
        {
            hamburgerBox.BackColor = Color.Transparent;
        }

        // If pushing the mouse button highlight in black
        private void hamburgerBox_MouseDown(object sender, MouseEventArgs e)
        {
            hamburgerBox.BackColor = Color.Black;
        }

        // Next is the question mark button handling.
        // remove the highlight once the button is clicked and display the help menu.
        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            questionButton.BackColor = Color.Transparent;
            helpPanel.Show();
            menuOut();
            helpPanel.BringToFront();
            HowToText.Width = 380;
            infoTextBox.Hide();
            HowToText.Show();
            helpTitle.Text = "Help: How to use L-JESS";

        }

        //if moving the mouse over, highlight in grey
        private void questionButton_MouseHover(object sender, EventArgs e)
        {
            questionButton.BackColor = Color.DarkGray;
        }

        //if moving the mouse away, go back to original trasparency
        private void questionButton_MouseLeave(object sender, EventArgs e)
        {
            questionButton.BackColor = Color.Transparent;
        }

        // If pushing the mouse button highlight in black
        private void questionButton_MouseDown(object sender, MouseEventArgs e)
        {
            questionButton.BackColor = Color.Black;
        }

        // Give a tooltip to help the user know what various buttons do.
        private void qMarkTip_Popup(object sender, PopupEventArgs e)
        {

        }

        // provide the different menui box optons and what happens if selected.
        // When a mode is selected, print the description text in the help panel
        // Also set the mode variable equal to that mode
        private void menuBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            dTitle.ForeColor = Color.White;
            switch (menuBox.SelectedItem.ToString())
            {
                case "Random Error Conditions":
                    dTitle.Text = menuBox.SelectedItem.ToString();
                    mode = "Random Error Conditions";
                    isRand = true;
                    description.Text = "Various error conditions will be chosen randomly to "
                        + "better simulate a real starting experience with unknown problems.";
                    break;
                case "No Oil Pressure":
                    mode = "No Oil Pressure";
                    isRand = false;
                    dTitle.Text = menuBox.SelectedItem.ToString();
                    description.Text = "No Oil Pressure Recovery: \n" +
                        "At any time after N2 reaches 20%, if the oil pressure indicator is not beginning to " +
                        "move, then there is a problem with the oil pump. To avoid damaging the engine, do " +
                        "not engage the fuel and discontinue start sequence.";
                    break;
                case "Starter Valve Sticks Open":
                    mode = "Starter Valve Sticks Open";
                    isRand = false;
                    dTitle.Text = menuBox.SelectedItem.ToString();
                    description.Text = "Starter Valve Stuck Open Procedure: \n" +
                        "At any time during normal start-up of the engine, if the starter valve sticks open " +
                        "(starter valve light on even though starter valve is supposedly closed), disengage engine " +
                        "and isolate bleed flow to the engine (i.e. close pneumatic cross feed valve.) In the simulation " +
                        "use 'x' to simulate this procedure.";
                    break;
                case "No Light Off- without Fuel Flow":
                    mode = "No Light Off- without Fuel Flow";
                    isRand = false;
                    dTitle.Text = menuBox.SelectedItem.ToString();
                    description.Text = "Handling No Light Off (without fuel flow): \n" +
                        "Once the fuel has been engaged to the engine, if light off does not occur " +
                        "(indicated by no fuel flow and/or no EGT rise), discontinue the start sequence and check the fuel.";
                    break;
                case "No Light Off- with Fuel Flow":
                    mode = "No Light Off- with Fuel Flow";
                    isRand = false;
                    dTitle.Text = menuBox.SelectedItem.ToString();
                    description.Text = "Handling No Light Off (with fuel flow): \n" +
                        "Once the fuel has been engaged to the engine, if light off does not occur " +
                        "(indicated by no EGT rise), discontinue the start sequence and check the fuel.";
                    break;
                case "No N1 Rotation":
                    mode = "No N1 Rotation";
                    isRand = false;
                    dTitle.Text = menuBox.SelectedItem.ToString();
                    description.Text = "No N1 Rotation Recovery: \n" +
                        "At any time after N2 reaches 20%, if N1 is not beginning to move, then there is a problem with " +
                        "the N1 turbine. To avoid damaging the engine, do not engage fuel flow and discontinue start sequence.";
                    break;
                case "Hung Start":
                    mode = "Hung Start";
                    isRand = false;
                    dTitle.Text = menuBox.SelectedItem.ToString();
                    description.Text = "Hung Start Prevention: \n" +
                        "At any time during normal start-up of the engine, if the engine stops " +
                        "increasing speed before normal RPM is reached, then a hung start has occurred. " +
                        "To counteract, disengage fuel flow and discontinue start sequence.";
                    break;
                case "Hot Start":
                    mode = "Hot Start";
                    isRand = false;
                    dTitle.Text = menuBox.SelectedItem.ToString();
                    description.Text = "Hot Start Prevention: \n" +
                        "At any time after you notice N2 rotation, if the EGT begins rising too quickly" +
                        " or it exceeds the top of the white range on the meter, disengage the fuel flow " +
                        "as quickly as possible to avoid damaging the engine. Continue spooling the engine " + 
                        "(starter pressed) until the engine reaches a safe temperature.";
                    break;
                case "No Error Conditions":
                    mode = "No Error Conditions";
                    isRand = false;
                    dTitle.Text = menuBox.SelectedItem.ToString();
                    description.Text = "This simulates an engine start with no issues.\n" +
                        " A normal start of the engine consists of the following steps:\n\n" +
                        " 1. Depress starter.\n\n 2. Once N2 is over 20%, toggle fuel valve.\n\n 3. Release starter between 33 – 48% N2.\n\n" +
                        " 4. Once engine reaches idle, toggle fuel valve again to power engine down.";
                    break;

            }
            settingButton.BackColor = Color.Transparent;
            settingHighlight.BackColor = description.BackColor;
        }

        // If the user selects the setting icon, expand the menu and remove highlight
        private void pictureBox1_Click_2(object sender, EventArgs e)
        {
            menuOut();
            settingButton.BackColor = Color.Transparent;
            settingHighlight.BackColor = description.BackColor;
        }

        // When hovering over the setting button highlight it in grey
        private void pictureBox1_MouseHover(object sender, EventArgs e)
        {
            settingButton.BackColor = Color.DarkGray;
            settingHighlight.BackColor = Color.DarkGray;
        }

        // when the mouse moves away from the setting button unhighlight.
        private void settingButton_MouseLeave(object sender, EventArgs e)
        {
            settingButton.BackColor = Color.Transparent;
            settingHighlight.BackColor = description.BackColor; 
        }

        // while mouse is pressed on the setting button highlight in black
        private void settingButton_MouseDown(object sender, MouseEventArgs e)
        {
            settingButton.BackColor = Color.Black;
            settingHighlight.BackColor = Color.Black;
        }

        // When the tutorial Mode button is clicked, change the tutorMode notifier
        // Also update the icon.
        private void pictureBox1_Click_3(object sender, EventArgs e)
        {
            tutorHandler();
        }

        // A method to handle the tutor mode actions
        void tutorHandler()
        {
            tutorMode = tutorMode * -1;
            tutorButton.BackColor = Color.Transparent;
            tutorHighlight.BackColor = description.BackColor;
            tutorialMode.BackColor = Color.Transparent;

            if (tutorMode > 0)
            {
                tutorButton.Image = EngineStartSimulator.Properties.Resources.tutorOn1;
                tutorialMode.ForeColor = Color.YellowGreen;
                tutorialMode.Text = "Tutorial Mode: ON";
            }
            else
            {
                tutorButton.Image = EngineStartSimulator.Properties.Resources.tutorOff1;
                tutorialMode.ForeColor = Color.White;
                tutorialMode.Text = "Tutorial Mode: OFF";
            }
        }

        // While hovering highlight in grey
        private void pictureBox1_MouseHover_1(object sender, EventArgs e)
        {
            tutorButton.BackColor = Color.DarkGray;
            tutorHighlight.BackColor = Color.DarkGray;
            tutorialMode.BackColor = Color.DarkGray;
        }

        // Unhighlight when the mouse leaves
        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            tutorButton.BackColor = Color.Transparent;
            tutorHighlight.BackColor = description.BackColor;
            tutorialMode.BackColor = Color.Transparent;
        }

        // when the mouse button is pushed, highlight in black 
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            tutorButton.BackColor = Color.Black;
            tutorHighlight.BackColor = Color.Black;
            tutorialMode.BackColor = Color.Black;
        }

        // Next we hae the pause play button
        // When pressed, signal that the simulator should either play or pause
        // Update the icon and text accordingly
        private void pictureBox1_Click_4(object sender, EventArgs e)
        {
            playPause = playPause * -1;
            pauseButton.BackColor = Color.Transparent;
            pauseHighlight.BackColor = description.BackColor;
            pauseLabel.BackColor = Color.Transparent;

            if (playPause < 0)
            {
                pauseButton.Image = EngineStartSimulator.Properties.Resources.play101;
                pauseLabel.Text = "Resume";
                guagePause();
            }
            else
            {
                pauseButton.Image = EngineStartSimulator.Properties.Resources.pause10;
                pauseLabel.Text = "Pause";
                guagePlay();
            }
        }

        // for the hovering be sure to highlight the button in gray
        private void pauseButton_MouseHover(object sender, EventArgs e)
        {
            pauseButton.BackColor = Color.DarkGray;
            pauseHighlight.BackColor = Color.DarkGray;
            pauseLabel.BackColor = Color.DarkGray;
        }

        // After moving the mouse away, remove the highlight.
        private void pauseButton_MouseLeave(object sender, EventArgs e)
        {
            pauseButton.BackColor = Color.Transparent;
            pauseHighlight.BackColor = description.BackColor;
            pauseLabel.BackColor = Color.Transparent;
        }

        // When the mouse is pressed on the icon, highlight in black
        private void pauseButton_MouseDown(object sender, MouseEventArgs e)
        {
            pauseButton.BackColor = Color.Black;
            pauseHighlight.BackColor = Color.Black;
            pauseLabel.BackColor = Color.Black;
        }

        // The Stop Button gets the same visual affects...
        private void stopButton_Click(object sender, EventArgs e)
        {
            stopButton.BackColor = Color.Transparent;
            stopHighlight.BackColor = description.BackColor;
            stopLabel.BackColor = Color.Transparent;
            guageStop();
        }

        // Hovering over stop button, highlight dark grey
        private void stopButton_MouseHover(object sender, EventArgs e)
        {
            stopButton.BackColor = Color.DarkGray;
            stopHighlight.BackColor = Color.DarkGray;
            stopLabel.BackColor = Color.DarkGray;
        }

        // leaviing the stop button, unhighlight
        private void stopButton_MouseLeave(object sender, EventArgs e)
        {
            stopButton.BackColor = Color.Transparent;
            stopHighlight.BackColor = description.BackColor;
            stopLabel.BackColor = Color.Transparent;
        }

        // When mouse clicks down highlight in black
        private void stopButton_MouseDown(object sender, MouseEventArgs e)
        {
            stopButton.BackColor = Color.Black;
            stopHighlight.BackColor = Color.Black;
            stopLabel.BackColor = Color.Black;
        }

        // And the restart Button gets the same visual affects too...
        private void restartButton_Click(object sender, EventArgs e)
        {
            restartButton.BackColor = Color.Transparent;
            restartLabel.BackColor = Color.Transparent;
            restartHighlight.BackColor = description.BackColor;
            initializeGauges();
            timer2.Stop();
        }

        // restart hover...
        private void restartButton_MouseHover(object sender, EventArgs e)
        {
            restartButton.BackColor = Color.DarkGray;
            restartLabel.BackColor = Color.DarkGray;
            restartHighlight.BackColor = Color.DarkGray;
        }

        // Restart button if mouse leaves
        private void restartButton_MouseLeave(object sender, EventArgs e)
        {
            restartButton.BackColor = Color.Transparent;
            restartLabel.BackColor = Color.Transparent;
            restartHighlight.BackColor = description.BackColor;
        }

        // when mouse is clicked down on restart button
        private void restartButton_MouseDown(object sender, MouseEventArgs e)
        {
            restartButton.BackColor = Color.Black;
            restartLabel.BackColor = Color.Black;
            restartHighlight.BackColor = Color.Black;
        }

        // NO!
        private void tutorHighlight_TextChanged(object sender, EventArgs e)
        {

        }

        // Redundant?
        private void tutorial(object sender, MouseEventArgs e)
        {
            tutorMode = tutorMode * -1;
            tutorButton.BackColor = Color.Transparent;
            tutorHighlight.BackColor = description.BackColor;
            tutorialMode.BackColor = Color.Transparent;

            if (tutorMode > 0)
            {
                tutorButton.Image = EngineStartSimulator.Properties.Resources.tutorOn1;
                tutorialMode.ForeColor = Color.YellowGreen;
                tutorialMode.Text = "Tutorial Mode: ON";
            }
            else
            {
                tutorButton.Image = EngineStartSimulator.Properties.Resources.tutorOff1;
                tutorialMode.ForeColor = Color.White;
                tutorialMode.Text = "Tutorial Mode: OFF";
            }

        }

        // NO!!
        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        // A method to handle moving the menu bar out and resizing all the items
        private void menuOut()
        {
            splitContainer1.SplitterDistance = 385;
            splitterLoc = 385;
            line1.Width = 385;
            line2.Width = 385;
            line3.Width = 385;
            description.Width = 330;
            dTitle.Width = 385;
            menuBox.Show();
            menuBox.Width = 322;
            settingHighlight.Width = 385;
            tutorHighlight.Width = 385;
            pauseHighlight.Width = 385;
            stopHighlight.Width = 385;
            restartHighlight.Width = 385;
            if (splitContainer1.Height > 520)
            {
                description.Show();
                dTitle.Show();
            }
        }

        // A method to handle putting the menu bar in 
        private void menuIn()
        {
            try
            {
                splitContainer1.SplitterDistance = 45;
            }
            catch(System.InvalidOperationException)
            {

            }
            //line1.Width = 45;
            splitterLoc = 45;
            description.Hide();
            dTitle.Hide();
            menuBox.Hide();
            helpPanel.Hide();
            //centerToggle(0);
        }

        // A double click in the gage panel will put the menu bar back in
        private void splitContainer1_Panel2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            menuIn();
        }

        // Double clicking the mouse will put the menu panel out
        private void splitContainer1_Panel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            menuOut();
        }

        // Info button at the bottom of the screen will pop up a window explaining about the production of the program
        // don't forget to update the icon appearance
        private void infoButton_Click(object sender, EventArgs e)
        {
            infoButton.BackColor = Color.Transparent;

            /*MessageBox.Show("     The LeTourneau Jet Engine Start Simulator (L-JESS) Version 1.0.0.0 (prototype) \n\n" +
                "    L-JESS provides a learning environment for jet engine starting procedures under\n\t\t              different circumstances.\n\n " +
                "      L-JESS developed by LeTourneau Software Engineering students Spring 2016\n\n" +
                "Special Thanks to:\n  >Professor Matt Poleman\n  >Jared Norgran", "L-JESS Information");  
                */
            HowToText.Hide();
            infoTextBox.Show();
            menuOut();
            helpPanel.BringToFront();
            infoTextBox.Width = 380;
            helpTitle.Text = "Information";
            helpPanel.Show();   
        }

        // When mouse goes down, highlight in black
        private void infoButton_MouseDown(object sender, MouseEventArgs e)
        {
            infoButton.BackColor = Color.Black;
        }

        // When the mouse hovers over, highlight in grey
        private void infoButton_MouseHover(object sender, EventArgs e)
        {
            infoButton.BackColor = Color.DarkGray;
        }

        // if the mouse leaves the info button, no longer highlight it
        private void infoButton_MouseLeave(object sender, EventArgs e)
        {
            infoButton.BackColor = Color.Transparent;
        }

        // as soon as the mouse comes over the info button highlight it in grey.
        private void infoButton_MouseEnter(object sender, EventArgs e)
        {
            infoButton.BackColor = Color.DarkGray;
        }

        // This method allows draghging to resize the splitter
        // If the user tries to move the splitter bigger, it will stay at a size of 385
        // If user tries to move it smaller, it will go to a size of 45
        private void splitContainer1_Panel1_Resize(object sender, EventArgs e)
        {
            if(splitterLoc == 385)
            { 
                if (splitContainer1.SplitterDistance > 385)
                 {
                    menuOut();
                 }
                 else if (splitContainer1.SplitterDistance < 385)
                 {
                        menuIn();
                  }
            }
            else if(splitterLoc == 45)
            {
                if(splitContainer1.SplitterDistance > 45)
                {
                    menuOut();
                }
                else if(splitContainer1.SplitterDistance < 45)
                {
                    menuIn();
                }
            }
        }

        // If the window size changes, make the menu bar shrink so there is no awkward auto resizing issues.
        private void mainWindow_SizeChanged(object sender, EventArgs e)
        {
            menuIn();
            relocateInfoButton();
        }

        // no
        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        // not needed now
        
        private void instructBox_MouseEnter(object sender, EventArgs e)
        {
            //if (instructBox.BackgroundImage.Equals(EngineStartSimulator.Properties.Resources.screenR))
            
                //instructBox.BackgroundImage = EngineStartSimulator.Properties.Resources.screen;
            
        }
        
        // If the mouse is over the panel, then flash the instruction box to red.
        private void splitContainer1_Panel2_MouseEnter(object sender, EventArgs e)
        {
            //instructBox.BackgroundImage = EngineStartSimulator.Properties.Resources.screenR;
        }

        // Make the instruction Box black when hovered over it.
        private void instructBox_MouseHover(object sender, EventArgs e)
        {
            //instructBox.BackgroundImage = EngineStartSimulator.Properties.Resources.screen;
        }
         

        // Put the menu in if user double clicks the gauge panels
        private void backgroundBox_DoubleClick(object sender, EventArgs e)
        {
            menuIn();
        }

        // Handle the splash screen time with a timer. After the timer finishes, hide the splash screen
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            splashImage.Hide();

        }

        // A back button to go back to the main menu after looking at instructions or info tabs.
        // Make the normal button operations and hide the help panel.
        private void backButton_Click(object sender, EventArgs e)
        {
            helpPanel.Hide();
            backButton.BackColor = Color.Transparent;
            backHighlight.BackColor = Color.Transparent;
        }

        // Don't forget clicking on the area around the button counts too.
        private void backHighlight_Click(object sender, EventArgs e)
        {
            helpPanel.Hide();
            backButton.BackColor = Color.Transparent;
            backHighlight.BackColor = Color.Transparent;
        }

        // When the mouse goes down turn the button background black.
        private void backButton_MouseDown(object sender, MouseEventArgs e)
        {
            backButton.BackColor = Color.Black;
            backHighlight.BackColor = Color.Black;
        }

        // Highlight the button in grey as soon as the mouse enters the area.
        private void backButton_MouseEnter(object sender, EventArgs e)
        {
            backButton.BackColor = Color.DarkGray;
            backHighlight.BackColor = Color.DarkGray;
        }

        // And keep the gray highlighting while the mouse hovers
        private void backButton_MouseHover(object sender, EventArgs e)
        {
            backButton.BackColor = Color.DarkGray;
            backHighlight.BackColor = Color.DarkGray;
        }

        // When mouse leaves, unhighlight
        private void backButton_MouseLeave(object sender, EventArgs e)
        {
            backButton.BackColor = Color.Transparent;
            backHighlight.BackColor = Color.Transparent;
        }

        // Don't need to do anything here :(
        private void guagePanel_Paint(object sender, PaintEventArgs e)
        {

        }

        // When the fuel slider button is clicked move it to the correct location.
        private void sliderButton_Click(object sender, EventArgs e)
        {
            if (mode.Equals("No Light Off- without Fuel Flow") || mode.Equals("No Light Off- with Fuel Flow"))
            {
                blockedFuel();
            }
            else
            {
                changeFuelButton();
            }
        }

        // This method makes sure that the fuel toggle is centered even with changing screen sizes 
        private void centerToggle(int raise)
        {
            int xLoc = 0;
            int yLoc = 0;

            xLoc = ((panel1.Width / 2) - (sliderButton.Width / 2) - (panel1.Width / 53));
            yLoc = ((panel1.Height / 2) + (panel1.Height / 5)) - (sliderButton.Height / 2) - raise;

            sliderButton.Location = new Point(xLoc, yLoc);

        }

        // Using the centering method, this method handles moving the toggle switch to its new locations.
        // Also handles some initial gauge movements
        private void changeFuelButton()
        {
            if (fuelOn == -1)
            {
                centerToggle((panel1.Height / 4));
            }
            else
            {
                centerToggle(0);
            }
            fuelOn = fuelOn * -1;

            // When fuel is toggled on some gauge changes take place
            if(fuelOn == 1 )
            {
                gauges[6].setSpeed(5);
                onState[6] = true;
                gauges[5].setSpeed(8);
                onState[5] = true;

                if (gauges[0].getPosition() <= 2)
                {
                    while (gauges[5].getPosition() < 42)
                    {
                        gauges[5].move();
                        if (gauges[6].getPosition() < 20)
                        {
                            gauges[6].move();
                        }
                    }
                    
                    gauges[4].setSpeed(1.2f);
                    onState[4] = true;

                    gauges[2].setSpeed(0.75f);

                    gauges[1].setSpeed(2);
                    onState[1] = true;     
                    
                }

                if (startButtonOn == 1 && gauges[0].getPosition() > 2 )
                {

                    gauges[1].setSpeed(4);
                    onState[1] = true;

                    gauges[4].setSpeed(1.2f);
                    onState[4] = true;

                    gauges[2].setSpeed(0.75f);
                }
            }
            //When toggled off make changes as well
            else if(fuelOn == -1 && gauges[0].getPosition() > 2)
            {
                System.GC.Collect();

                gauges[1].setSpeed(.7f);
                onState[1] = true;

                gauges[0].setSpeed(1.5f);
                onState[0] = true;
                gauges[2].setSpeed(.5f);
                onState[2] = true;
                gauges[3].setSpeed(1.3f);
                onState[3] = true;

                gauges[6].setSpeed(5);
                onState[6] = true;
                gauges[5].setSpeed(8);
                onState[5] = true;
                gauges[4].setSpeed(.6f);
                onState[4] = true;
            }
            else if (gauges[0].getPosition() <= 2)
            {
                System.GC.Collect();

                while (gauges[5].getPosition() > 0)
                {
                    gauges[5].moveBack();
                    if (gauges[6].getPosition() > 0)
                    {
                        gauges[6].moveBack();
                    }
                }

                if(gauges[5].getPosition() < 0)
                {
                    onState[5] = false;
                }
                if (gauges[6].getPosition() < 0)
                {
                    onState[6] = false;
                }
                
                gauges[1].setSpeed(1);
                onState[1] = false;

                
                onState[4] = false;

                gauges[2].setSpeed(0.5f);
            }
        }

        // When the start valve is clicked, register that in the startButtonOn variable
        // And take care of the animation. 
        // This turns off the valve
        private void startValve_Click(object sender, EventArgs e)
        {
            changeStartValve();
            attemptOff++;
        }

        private void changeStartValve()
        {
            attemptOff++;

            if (mode != "Starter Valve Sticks Open" || gauges[0].getPosition() < 3 || disengaged)
            {
                startButtonOn = startButtonOn * -1;
                guageReverser();


                if (startButtonOn == -1)
                {
                    startValve.Image = EngineStartSimulator.Properties.Resources.start_valveN;
                    engineStart.Image = EngineStartSimulator.Properties.Resources.LightOff2;
                }
                else if (startButtonOn == 1)
                {
                    startValve.Image = EngineStartSimulator.Properties.Resources.start_valve_down2;
                    engineStart.Image = EngineStartSimulator.Properties.Resources.LightOn;
                    if (goTime == 0)
                    {
                        timer2.Start();
                    }
                    goTime++;
                }
            }
            
            
        }

        // Nope...
        private void splitContainer1_Click(object sender, EventArgs e)
        {
            
        }

        // This covers moving the menu back in when double clicking on the main screen
        // This method is used by all guages.
        private void n2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            menuIn();
        }

        // When the start valve is double clicked, register that in the startButtonOn variable
        // And take care of the animation, keeping it down this time.
        private void startValve_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            changeStartValve();
        }

        // This turns on the valve
        // while the mouse is held down, the valve stays open
        // register that in the startButtonOn variable.
        private void startValve_MouseDown(object sender, MouseEventArgs e)
        {
            changeStartValve();
            
            //lines();
        }

        // This does nothing
        private void sliderButton_DragLeave(object sender, EventArgs e)
        {
            //changeFuelButton();
        }

        // Also does nothing...
        private void sliderButton_DragDrop(object sender, DragEventArgs e)
        {
            //changeFuelButton();
        }

        // Some hopeful ideas to creat guage needle animations.
        System.Drawing.Pen myPen;

        private void lines()
        {
            myPen = new System.Drawing.Pen(System.Drawing.Color.Red);
            System.Drawing.Graphics formGraphics = this.CreateGraphics();
            formGraphics.DrawLine(myPen, 0, 0, 200, 200);
            myPen.Dispose();
            formGraphics.Dispose();
        }

        private void relocateInfoButton()
        {
            int xLoc = 0;
            int yLoc = 0;

            yLoc = (splitContainer1.Height * 2) -9;
            xLoc = 2;

            infoButton.Location = new Point(xLoc, yLoc);
           
        }

        // A method to center the splash image
        private void centerSplashImage()
        {
            int xLoc = 0;
            int yLoc = 0;

            yLoc = splitContainer1.Height * 5 / 8;
            //xLoc = (splitContainer1.Width / 2) + 10 * (splashImage.Width / 2) / 3;
            xLoc = (splitContainer1.Width / 2) + 5*(splashImage.Width / 2)/2;

            splashImage.Location = new Point(xLoc, yLoc);

            yLoc = (splitContainer1.Height / 2) + splashImage.Height + 15;
            //xLoc = ((splitContainer1.Width / 2) +  (splashMessage.Width / 2)) *3 / 7;
            xLoc = ((splitContainer1.Width / 2) + (splashMessage.Width / 2));

            splashMessage.Location = new Point(xLoc, yLoc);

        }

        private void mainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ControlKey:
                    changeStartValve();
                    attemptOff++;
                    break;
                case Keys.Space:
                    if (mode.Equals("No Light Off- without Fuel Flow") || mode.Equals("No Light Off- with Fuel Flow"))
                    {
                        blockedFuel();
                    }
                    else
                    {
                        changeFuelButton();
                    }
                    break;
                case Keys.P:
                    playPause = playPause * -1;
                    if (playPause < 0)
                    {
                        pauseButton.Image = EngineStartSimulator.Properties.Resources.play101;
                        pauseLabel.Text = "Resume";
                        guagePause();
                    }
                    else
                    {
                        pauseButton.Image = EngineStartSimulator.Properties.Resources.pause10;
                        pauseLabel.Text = "Pause";
                        guagePlay();
                    }                    
                    break;
                case Keys.R:
                    break;
                case Keys.S:
                    guageStop();
                    break;
                case Keys.M:
                    if (splitContainer1.SplitterDistance == 385)
                    {
                        menuIn();
                    }
                    else
                    {
                        menuOut();
                    }
                    break;
                case Keys.I:
                    HowToText.Hide();
                    infoTextBox.Show();
                    menuOut();
                    helpPanel.BringToFront();
                    infoTextBox.Width = 380;
                    helpTitle.Text = "Information";
                    helpPanel.Show(); 
                    break;
                case Keys.Q:
                    helpPanel.Show();
                    menuOut();
                    helpPanel.BringToFront();
                    HowToText.Width = 380;
                    infoTextBox.Hide();
                    HowToText.Show();
                    helpTitle.Text = "Help: How to use L-JESS";

                    break;
                case Keys.T:
                    break;
                case Keys.X:
                    disengage();
                    break;
            }
     

        }

        private void mainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
            {
                //changeStartValve();
            }
            else
            {
                //e.SuppressKeyPress = true;
            }

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //Timer controled Guage movement
            // this is one thing that needs to appear in timerTick
            memDump++;
            if(memDump > 40)
            {
                System.GC.Collect();
                memDump = 0;
            }

            switch( mode)
            {
                case "No Error Conditions":
                    noError();
                    break;
                case "Hung Start":
                    hungStart();
                    break;
                case "Hot Start":
                    hotStart();
                    break;
                case "No N1 Rotation":
                    noN1();
                    break;
                case "No Oil Pressure":
                    noOP();
                    break;
                case "Starter Valve Sticks Open":
                    startValveStuck();
                    break;
                case "No Light Off- with Fuel Flow":
                    noLight();
                    break;
                case "No Light Off- without Fuel Flow":
                    noLightNoFuel();
                    break;
                case "Random Error Conditions":
                    RandomMode();
                    break;
            }
            
            // When the starter is pressed and no errors have occurred
            if (startButtonOn == 1 && !errorOccured)
            {
                for (int i = 0; i < gauges.Length; i++)
                {
                    if (onState[i] == true)
                        gauges[i].move();
                }
            }
            // If the start button and fuel button are released, or a user error occurred, or the engine is below 10, then move the guages backwards
            else if((startButtonOn == -1 && fuelOn == -1) || errorOccured || (startButtonOn == -1 && fuelOn == 1 && gauges[0].getPosition() <= 10) )
            {
                for (int i = 0; i < gauges.Length; i++)
                {
                    if (onState[i] == true)
                        gauges[i].moveBack();
                }
            }

            // When fuel is on, starter is off and no error, then continue to increase the gauges.
            else if (startButtonOn == -1 && fuelOn == 1 && goTime > 0 && gauges[0].getPosition() > 10 && !errorOccured)
            {
                for (int i = 0; i < gauges.Length; i++)
                {
                    if (onState[i] == true)
                        gauges[i].move();
                }
            }

        }

        void guageReverser()
        {
            direction = direction * -1;
        }

        // Method for pausing the simulation without losing current place
        public void guagePause()
        {
            timer2.Stop();
        }

        // Method for resuming the simulation right where it left off
        public void guagePlay()
        {
            timer2.Start();
        }

        // Method for restarting the simulation in the same mode
        public void guageRestart()
        {
            goTime = 1;
            timer2.Stop();

            for (int i = 0; i < 7; i++)
            {
                onState[i] = false;
                gauges[i].setPosition(-1);
                gauges[i].move();
            }

            stopped();
            firstStopped = 0;

            timer2.Start();
            
        }

        // Method for stopping simulation and reseting to default mode
        public void guageStop()
        {
            //fuelOn = -1;
            //startButtonOn = -1;
            goTime = 1;
            mode = "No Error Conditions";

            timer2.Stop();
                        
            for (int i = 0; i < 7; i++)
            {
                onState[i] = false;
                gauges[i].setPosition(-1);
                gauges[i].move();
            }

            stopped();
            firstStopped = 0;

            timer2.Start();
            //onState[0] = true;

        }

        public void tenPercentMove()
        {
            // Start the other gauges when N2 reaches 10%
            if (gauges[0].getPosition() > 32)
            {
                if (!mode.Equals("No N1 Rotation"))
                {
                    onState[3] = true;
                }
                if (!mode.Equals("No Oil Pressure"))
                {
                    onState[2] = true;
                }
            }
        }

        public void fuelToggledOnStop()
        {
            if (fuelOn == 1 || fuelOnBlocked == 1)
            {
                // Control the stopping of the guages during fuel switch toggle
                if (gauges[6].getPosition() > 20 && (fuelOn == 1 || fuelOnBlocked == 1))
                    onState[6] = false;

                if (gauges[5].getPosition() > 42 && (fuelOn == 1 || fuelOnBlocked == 1))
                    onState[5] = false;

                if (gauges[4].getPosition() > 21 && (fuelOn == 1 || fuelOnBlocked == 1))
                    onState[4] = false;
            }
        }

        public void egtMotion()
        {
            // Jump up, slow and steady the EGT gauge
            if (gauges[1].getPosition() > 143 && onState[1] == true)
            {
                gauges[1].setSpeed(1.4f);
                onState[1] = false;

            }
            else if (gauges[1].getPosition() > 137 && onState[1] == true)
            {
                gauges[1].setSpeed(1);
            }
            else if (gauges[1].getPosition() > 127 && onState[1] == true)
            {
                gauges[1].setSpeed(2);
            }
            else if (gauges[1].getPosition() > 120 && onState[1] == true)
            {
                gauges[1].setSpeed(3);
            }
            else if (gauges[1].getPosition() > 110 && onState[1] == true)
            {
                gauges[1].setSpeed(4);
            }
            else if (gauges[1].getPosition() > 90 && onState[1] == false)
            {
                gauges[1].moveBack();
            }
        }

        public void oilPressureMotion()
        {
            // Slow and Stop Oil Pressure gauge
            if (gauges[2].getPosition() > 78 && fuelOn == 1)
            {
                onState[2] = false;
            }
            else if (gauges[2].getPosition() > 72 && fuelOn == 1)
            {
                gauges[2].setSpeed(.2f);
            }
            else if (gauges[2].getPosition() > 65 && fuelOn == 1)
            {
                gauges[2].setSpeed(.5f);
            }
            else if (gauges[2].getPosition() > 55 && fuelOn == 1)
            {
                gauges[2].setSpeed(1f);
            }
        }

        public void steadyToIdle()
        {
            // Slow to steady running condition
            if (gauges[0].getPosition() > 165 && fuelOn == 1 && startButtonOn == -1)
            {
                for (int i = 0; i < 7; i++)
                {
                    onState[i] = false;
                }

                if (tutorMode == 1 && errorHandled < 2)
                {
                    timer2.Stop();
                    MessageBox.Show("You have successfully started the engine and brought it to an idle. " +
                        "To power down, toggle the fuel off and let the engine spool down completely before attempting another start. " +
                        "", "Tutorial Message");
                    timer2.Start();
                    errorHandled = 2;
                }
                else if (tutorMode == -1 && errorHandled < 2)
                {
                    errorHandled = 2;
                }
            }
            else if (gauges[0].getPosition() > 160 && fuelOn == 1 && startButtonOn == -1)
            {
                gauges[0].setSpeed(.2f);
                gauges[3].setSpeed(.2f);
            }
            else if (gauges[0].getPosition() > 150 && fuelOn == 1 && startButtonOn == -1)
            {
                gauges[0].setSpeed(.5f);
                gauges[3].setSpeed(.5f);
            }
            else if (gauges[0].getPosition() > 135 && fuelOn == 1 && startButtonOn == -1)
            {
                gauges[0].setSpeed(1);
                gauges[3].setSpeed(1);
            }
            
        }

        // If the starter is held on past 40%
        public void starterPast()
        {
            if(startButtonOn == 1 && gauges[0].getPosition() > 125 && tutorMode == 1)
            {
                timer2.Stop();
                MessageBox.Show("Running the Starter past 40% of N2 can be very harmful to the starter and " +
                    "may result in the need for a timely and costly repair. Release the start valve and let the engine come to a stop.", "Tutorial Warning");
                changeStartValve();
                disengage();
                if (fuelOn == 1)
                {
                    changeFuelButton();
                }
                timer2.Start();
                errorOccured = true;
            }
            else if (startButtonOn == 1 && gauges[0].getPosition() > 130 && tutorMode == -1)
            {
                timer2.Stop();
                MessageBox.Show("You have run the starter past 40% of N2 which resulted in the destruction " +
                    "of the starter. Release the start valve and let the engine stop, this plane is now out of commission.", "STARTER FAILURE!");
                changeStartValve();
                if (fuelOn == 1)
                {
                    changeFuelButton();
                }
                timer2.Start();
                errorOccured = true;
            }
        }

        // If the starter gets pressed while the engine is in motion
        public void starterRepress()
        {
            if (startButtonOn == 1 && direction == -1 && gauges[0].getPosition() > 1 && tutorMode == 1 && goTime > 1)
            {
                timer2.Stop();
                MessageBox.Show("Attempting to re-engage the starter after it has been released is very harmful to the starter " +
                    " and may result in the need for a timely and costly repair. Release the start valve and let the engine come " +
                    "to a stop before re-engaging the starter.", "Tutorial Warning");
                changeStartValve();
                if (fuelOn == 1)
                {
                    changeFuelButton();
                }
                timer2.Start();
                errorOccured = true;

            }
            else if (startButtonOn == 1 && direction == -1 && gauges[0].getPosition() > 8 && tutorMode == -1 && goTime > 1)
            {
                timer2.Stop();
                MessageBox.Show("You have destroyed the starter by trying to engage it while the engine was still moving! " +
                    "Release the start valve and let the engine stop, this plane is now out of commission.", "STARTER FAILURE!");
                changeStartValve();
                if (fuelOn == 1)
                {
                    changeFuelButton();
                }
                timer2.Start();
                errorOccured = true;

            }
            else if (startButtonOn == 1 && direction == -1 && gauges[0].getPosition() > 1 && tutorMode == -1 && goTime > 1)
            {
                timer2.Stop();
                MessageBox.Show("You caused starter grinding by trying to engage it while the engine was still moving! " +
                    "That was a close call to damaging the starter", "STARTER GRIND!");
                changeStartValve();
                if (fuelOn == 1)
                {
                    changeFuelButton();
                }
                timer2.Start();
                errorOccured = true;
            }
        }

        // Resets the gauges (speed and onState) once they have stopped
        public void stopped()
        {
            if(firstStopped < 2)
            {
                firstStopped++;
            }

            if(gauges[0].getPosition() <= 1)
            {

                if(mode.Equals("No Error Conditions") && errorHandled == 2)
                {
                    timer2.Stop();
                    MessageBox.Show("A perfect start -> idle -> power down with no errors! Great Job! " +
                        "", "WELL DONE!");
                    timer2.Start();
                    errorHandled = 0;
                }

                disengaged = false;
                attemptOff = 0;
                    errorHandled = 0;
                    goFinished = 0;
                    goTime = 1;
                    gauges[0].setSpeed(1.5f);
                    onState[0] = true;
                    gauges[1].setSpeed(1f);
                    onState[1] = false;
                    gauges[2].setSpeed(1f);
                    onState[2] = false;
                    gauges[3].setSpeed(1.5f);
                    onState[3] = false;

                    gauges[6].setSpeed(1);
                    onState[6] = false;
                    gauges[5].setSpeed(1);
                    onState[5] = false;
                    gauges[4].setSpeed(1);
                    onState[4] = false;
                    flood = 0;
                    lastPos = 0;

                    gauges[1].setPosition(5);


                    if (fuelOn == 1  && firstStopped == 1)
                    {
                        changeFuelButton();
                    }
                    if(fuelOnBlocked == 1 && firstStopped == 1)
                    {
                        blockedFuel();
                    }
                    if (startButtonOn == 1 && firstStopped == 1)
                    {
                        changeStartValve();
                    }

                    errorOccured = false;
                if (isRand)
                {
                    RandomMode();
                }
            }
        }

        // If the fuel is toggled on too early, this will go into a hot start mode.
        public void earlyFuel()
        {
            if (gauges[0].getPosition() < 70 && fuelOn == 1 && tutorMode == 1 )
            {
                timer2.Stop();
                MessageBox.Show("You started fuel flow too early. preliminary fuel flow can " +
                    "cause the engine to enter a hot start which may result in failure of costly engine components.", "Tutorial Warning");
                changeFuelButton();
                if (startButtonOn == 1)
                {
                    changeStartValve();
                }
                timer2.Start();
            }
            else if ( fuelOn == 1 && flood > 0 && tutorMode == 1 && gauges[1].getPosition() > 122)
            {
                timer2.Stop();
                MessageBox.Show("Notice the rapid increase in the EGT with no sign of slowing down: Some error in the engine has caused a hot Start  " +
                    " which may result in failure of costly engine components. Toggle the fuel off and abort the start procedure. This engine needs to" +  
                    " be inspected and/or repaired by a technician.", "Tutorial Warning");
                changeFuelButton();
                if (startButtonOn == 1)
                {
                    changeStartValve();
                }
                timer2.Start();
            }
            else if (gauges[0].getPosition() < 10 && fuelOn == 1)
            {
                flood = 2;
                onState[1] = true;
            }
            else if (gauges[0].getPosition() < 70 && fuelOn == 1)
            {
                flood = 1;
                onState[1] = true;
            }
        }

        // The hotStart that results from the early fuel introduction
        public void earlyFuelFail()
        {
            
            if(flood > 0 && fuelOn == 1)
            {
                if (gauges[1].getPosition() > 201)
                {
                    gauges[1].setSpeed(2);
                    errorHandled = -1;
                    timer2.Stop();
                    MessageBox.Show("The engine has engaged in a hot start and has reached temperatures that have destroyed costly engine components. " +
                        "Abort the start procedure as this plane will need major engine repair.", "HOT START!");
                    if (fuelOn == 1)
                    {
                        changeFuelButton();
                    }
                    if (startButtonOn == 1)
                    {
                        changeStartValve();
                    }
                    errorOccured = true;
                    timer2.Start();
                }
                else if (gauges[1].getPosition() > 140 )
                {
                    gauges[1].setSpeed(3);
                }
                else if (gauges[1].getPosition() > 34)
                {
                    gauges[1].setSpeed(5);
                }
                else if (gauges[1].getPosition() > 27 )
                {
                    gauges[1].setSpeed(4);
                }
                else if (gauges[1].getPosition() > 20 )
                {
                    gauges[1].setSpeed(3);
                }
                else if (gauges[1].getPosition() > 13 )
                {
                    gauges[1].setSpeed(2);
                }
                else if (gauges[1].getPosition() > 7 )
                {
                    gauges[1].setSpeed(1.5f);
                }
                else if (gauges[1].getPosition() > 1 )
                {
                    gauges[1].setSpeed(1);
                }
            }
            else if(flood == 0 && fuelOn == 1)
            {
                egtMotion();
            }
            else if(flood > 0 && fuelOn != 1 && startButtonOn != 1)
            {
                gauges[1].setSpeed(3.15f);
                onState[1] = true;
            }

            if(gauges[1].getPosition() > lastPos )
            {
                lastPos = gauges[1].getPosition();
                if (lastPos > 165 && errorOccured == false)
                {
                    errorHandled = 2;
                }
                else if(lastPos > 40 && errorOccured == false)
                {
                    errorHandled = 1;
                }
            }
        }

        public void avoidHS()
        {

            if (gauges[0].getPosition() < 3 && flood > 0 && errorHandled > 0 && errorOccured == false)
            {
                
                timer2.Stop();
                if (errorHandled == 1)
                {
                    MessageBox.Show("You successfully avoided a hot start situation saving the engine from damage and a costly repair. " +
                        "", "WELL DONE!");
                }
                else if(errorHandled == 2)
                {
                    MessageBox.Show("You avoided a hot start situation, however the engine still reached a high temerature  and should " +
                        "be inspected by a qualified technician.", "SO CLOSE!");
                }
                if (fuelOn == 1)
                {
                    changeFuelButton();
                }
                if (startButtonOn == 1)
                {
                    changeStartValve();
                }
                errorHandled = 0;
                flood = 0;
                timer2.Start();
            }
        }

        // Method for the Hung Start Condition
        public void hungStart()
        {
            //check possible errors
            starterPast();
            starterRepress();

            // move the gauges
            tenPercentMove();

            fuelToggledOnStop();

            earlyFuel();
            if (flood > 0)
            {
                onState[1] = true;
                gauges[1].setSpeed(1.6f);
            }


            stopped();
            oilPressureMotion();

            // Induce the hung start error
            if (gauges[0].getPosition() > 120 && errorHandled == 0)
            {
                errorHandled = -1;
                timer2.Stop();
                MessageBox.Show("The engine has engaged in a hung start situation, but due to continued attempts to start " +
                    "the engine, damage has been incurred. Abort the start procedure as this plane will need engine repair.", "ENGINE DAMAGE!");
                if (fuelOn == 1)
                {
                    changeFuelButton();
                }
                if (startButtonOn == 1)
                {
                    changeStartValve();
                }
                gauges[0].setSpeed(1.5f);
                gauges[3].setSpeed(1.3f);
                gauges[2].setSpeed(.5f);
                gauges[1].setSpeed(.85f);
                flood = 0;
                errorOccured = true;
                timer2.Start();
            }
            else if(gauges[0].getPosition() > 100 && errorHandled == 0)
            {
                gauges[0].setSpeed(.2f);
                gauges[3].setSpeed(.2f);
                gauges[2].setSpeed(.1f);
                gauges[1].setSpeed(.3f);
                
            }
            else if (gauges[0].getPosition() > 91 && errorHandled == 0)
            {
                gauges[0].setSpeed(.5f);
                gauges[3].setSpeed(.5f);
                gauges[2].setSpeed(.1f);
                gauges[1].setSpeed(.6f);
                

            }
            else if (gauges[0].getPosition() > 80 && errorHandled == 0)
            {
                gauges[0].setSpeed(1f);
                gauges[3].setSpeed(1f);
                gauges[2].setSpeed(.1f);
                gauges[1].setSpeed(1.2f);
                goFinished = 1;
                
            }

            // If the tutorial mode is on, give a warning earlier than failure
            if (gauges[0].getPosition() > 100 && tutorMode == 1)
            {
                timer2.Stop();
                MessageBox.Show("Notice that the engine is slowly increasing in speed in an unnatural manner. " +
                    "This indicates a hung start. Release the starter valve, cut fuel flow and have the engine inspected or repaired by a technician.", "Tutorial Warning");
                if (fuelOn == 1)
                {
                    changeFuelButton();
                }
                if (startButtonOn == 1)
                {
                    changeStartValve();
                }
                timer2.Start();
            }

            else if (gauges[0].getPosition() < 141 && startButtonOn == -1 && fuelOn == -1 && errorHandled == 0 && goFinished > 0)
            {
                errorHandled = 1;
                gauges[0].setSpeed(1.5f);
                gauges[3].setSpeed(1.3f);
                gauges[2].setSpeed(.85f);
            }

            if(errorHandled == 1 && gauges[0].getPosition() < 3)
            {
                timer2.Stop();
                errorHandled = 0;
                MessageBox.Show("You successfully recovered from a hung start avoiding further damage to the engine. " +
                    "Have the plane examined by FAA certified mechanics to find the cause of the hung start.", "WELL DONE!");
                timer2.Start();
            }
        }

        // The Hot Start Method
        public void hotStart()
        {
            if (goTime < 2)
            {
                flood = 2;
            }
            
            noError();
        }

        public void noN1()
        {
            noError();

            if (gauges[0].getPosition() > 65 && tutorMode == 1)
            {
                timer2.Stop();
                MessageBox.Show("Notice that N1 has not started rotating even though N2 has" + 
                    "passed 20%. This indicates an engine issue which " +
                    "needs to be adressed immediately by a technician. In a no N1 rotation " + 
                    "situation do not start fuel flow, release the starter valve, and have the " + 
                    "engine inspected or repaired.", "Tutorial Warning");
                if (fuelOn == 1)
                {
                    changeFuelButton();
                }
                if (startButtonOn == 1)
                {
                    changeStartValve();
                }
                timer2.Start();
            }

            if (gauges[0].getPosition() > 95)
            {
                timer2.Stop();
                errorOccured = true;
                if (fuelOn == -1)
                {
                    MessageBox.Show("N1 did not start rotating even though N2 has passed 30%. " +
                        "Due to persisted attempts to start the engine further damage has occurred!" +
                        " Abort the start procedure, this plane is now out of commission and needs " +
                        "to be repaired by a qualified technician.", "ENGINE DAMAGED!");
                }
                if (fuelOn == 1)
                {
                    MessageBox.Show("N1 did not start rotating even though N2 has passed 30%. " +
                        "Due to persisted attempts to start the engine and fuel flow introduced " +
                        "the engine has undergone serious damage! " +
                        "Abort the start procedure, this plane is now out of commission and needs " +
                        "to be repaired by an FAA certified technician, preferably Jared Norgran.", "ENGINE DAMAGED!");
                    changeFuelButton();
                }
                if (startButtonOn == 1)
                {
                    changeStartValve();
                }
                timer2.Start();
            }

            if (gauges[0].getPosition() > lastPos)
            {
                lastPos = gauges[0].getPosition();
                if (lastPos > 62 && errorOccured == false)
                {
                    errorHandled = 1;
                }
            }

            if (errorHandled == 1 && gauges[0].getPosition() < 3 && errorOccured == false)
            {
                timer2.Stop();
                errorHandled = 0;
                MessageBox.Show("You successfully recovered from a no N1 rotation situation avoiding further damage to the engine. " +
                    "Have the plane examined by FAA certified mechanics to find the cause of no N1 rotation.", "WELL DONE!");
                timer2.Start();
            }
        }

        // The method to handle a no oil pressure start simulation
        public void noOP()
        {
            noError();

            if (gauges[0].getPosition() > 65 && tutorMode == 1)
            {
                timer2.Stop();
                MessageBox.Show("Notice that Oil Pressure has not started building even though N2 has" +
                    "passed 20%. This indicates an engine issue which " +
                    "needs to be adressed immediately by a technician. In a no oil pressure " +
                    "situation do not start fuel flow, release the starter valve, and have the " +
                    "engine inspected or repaired immediately.", "Tutorial Warning");
                if (fuelOn == 1)
                {
                    changeFuelButton();
                }
                if (startButtonOn == 1)
                {
                    changeStartValve();
                }
                timer2.Start();
            }

            if (gauges[0].getPosition() > 120)
            {
                timer2.Stop();
                errorOccured = true;
                if (fuelOn == -1)
                {
                    MessageBox.Show("Oil pressure did not start building even though N2 is well passed 30%. " +
                        "Due to persisted attempts to start the engine further damage has occurred!" +
                        " Abort the start procedure, this plane is now out of commission and needs " +
                        "to be repaired by an FAA certified technician.", "ENGINE DAMAGED!");
                }
                if (fuelOn == 1)
                {
                    MessageBox.Show("Oil pressure did not start rotating even though N2 is well passed 30%. " +
                        "Due to persisted attempts to start the engine and fuel flow introduced " +
                        "the engine has undergone serious damage! " +
                        "Abort the start procedure, this plane is now out of commission and needs " +
                        "to be repaired by a qualified technician.", "ENGINE DAMAGED!");
                    changeFuelButton();
                }
                if (startButtonOn == 1)
                {
                    changeStartValve();
                }
                timer2.Start();
            }

            if (gauges[0].getPosition() > lastPos)
            {
                lastPos = gauges[0].getPosition();
                if (lastPos > 62 && errorOccured == false)
                {
                    errorHandled = 1;
                }
            }

            if (errorHandled == 1 && gauges[0].getPosition() < 3 && errorOccured == false)
            {
                timer2.Stop();
                errorHandled = 0;
                MessageBox.Show("You successfully recovered from a no oil pressure situation avoiding further damage to the engine. " +
                    "Have the plane examined by FAA certified mechanics to find the cause of no oil pressure.", "WELL DONE!");
                timer2.Start();
            }
        }

        public void startValveStuck()
        {
            noError();

            if(attemptOff > 0 && tutorMode == 1 && lastPos == 0)
            {
                lastPos = gauges[0].getPosition();
            }

            if(gauges[0].getPosition() > (lastPos+15) && attemptOff > 1 && tutorMode == 1)
            {
                timer2.Stop();
                MessageBox.Show("Notice that the start indicator light has not turned off even though the start valve has" +
                    "been released. This indicates the start valve is stuck open. " +
                    "In this situation disengage the engine and isolate bleed flow to the engine " +
                    "(i.e. close pneumatic cross feed valve.) In the simulation use 'x' to simulate this procedure. " +
                    "Then have the " +
                    "engine inspected or repaired immediately by qualified technicians.", "Tutorial Warning");

                disengage();
                timer2.Start();
            }

            if (errorHandled > 0  && gauges[0].getPosition() < 3 && errorOccured == false)
            {
                timer2.Stop();
                errorHandled = 0;
                MessageBox.Show("You successfully recovered from a starter Valve stuck open situation avoiding further damage to the engine. " +
                    "Have the plane examined by FAA certified mechanics to find the cause of the sticking starter valve.", "WELL DONE!");
                timer2.Start();
            }

            if (startButtonOn == 1 && gauges[0].getPosition() > 127 )
            {
                timer2.Stop();
                MessageBox.Show("Due to an unhandled stuck start valve the starter and other engine components have incurred damage! " +
                    " You should have disengaged the engine and isolated bleed flow to the engine " +
                    "(i.e. close pneumatic cross feed valve.) In the simulation use 'x' to simulate this procedure. " +
                    "This plane now needs immediate mechanical repair.", "ENGINE DAMAGED");
                disengage();
                errorOccured = true;
                timer2.Start();
            }
        }

        // A no light off with fuel flow method
        public void noLight()
        {
            //check possible errors
            starterPast();
            starterRepress();

            onState[1] = false;

            // if the scenario has ended, reset things.
            stopped();
            avoidHS();

            // move the gauges
            tenPercentMove();

            fuelToggledOnStop();

            oilPressureMotion();

            if (gauges[0].getPosition() > 127)
            {
                timer2.Stop();
                MessageBox.Show("Due to an unhandled No Light Off error the starter and other engine components have incurred damage!" +
                    " Abort the start procedure immediately. The new damages as well as the source of the problem must be repaired by a qualified technician  " +
                    "before this plane can be used again.", "ENGINE DAMAGED");
                errorOccured = true;
                if (fuelOnBlocked == 1)
                {
                    blockedFuel();
                }
                if (startButtonOn == 1)
                {
                    changeStartValve();
                }
                timer2.Start();
            }

            lastPos = gauges[0].getPosition();
            if (lastPos > 62 && errorOccured == false)
            {
                errorHandled = 1;
            }

            if (gauges[0].getPosition() < 3 && errorOccured == false && errorHandled == 1)
            {
                timer2.Stop();
                errorHandled = 0;
                MessageBox.Show("You successfully recovered from a no light off with fuel flow situation avoiding further damage to the engine. " +
                    "Have the plane examined by FAA certified mechanics to find whats preventing light off.", "WELL DONE!");
                timer2.Start();
            }

            if ((gauges[0].getPosition() > 110 || (gauges[0].getPosition() > 65 && direction == -1)) && tutorMode == 1)
            {
                timer2.Stop();
                MessageBox.Show("Notice that the engine did not light off when fuel was toggled on. Since there is fuel flow the problem " +
                    "is likely internal. Release the starter valve and abort the start procedure to avoid engine damage. " +
                    "The problem needs to be adressed immediately by a qualified technician. " +
                    "", "Tutorial Warning");
                if (fuelOnBlocked == 1)
                {
                    blockedFuel();
                }
                if (startButtonOn == 1)
                {
                    changeStartValve();
                }
                timer2.Start();
            }

        }

        // The method for the no Light off with no fuel flow
        public void noLightNoFuel()
        {
            //check possible errors
            starterPast();
            starterRepress();
            
            onState[1] = false;

            // if the scenario has ended, reset things.
            stopped();
            
            // move the gauges
            tenPercentMove();
            
            fuelToggledOnStop();

            oilPressureMotion();

            if (gauges[0].getPosition() > 127)
            {
                timer2.Stop();
                MessageBox.Show("Due to an unhandled No Light Off error the starter and other engine components have incurred damage!" +
                    " Abort the start procedure immediately. The new damages as well as the source of the problem must be repaired by a qualified technician  " +
                    "before this plane can be used again.", "ENGINE DAMAGED");
                errorOccured = true;
                if (fuelOnBlocked == 1)
                {
                    blockedFuel();
                }
                if (startButtonOn == 1)
                {
                    changeStartValve();
                }
                timer2.Start();
            }

            lastPos = gauges[0].getPosition();
            if (lastPos > 62 && errorOccured == false)
            {
                errorHandled = 1;
            }

            if (gauges[0].getPosition() < 3 && errorOccured == false && errorHandled == 1)
            {
                timer2.Stop();
                errorHandled = 0;
                MessageBox.Show("You successfully recovered from a no light off without fuel flow situation avoiding further damage to the engine. " +
                    "Have the plane examined by FAA certified mechanics to find whats stopping fuel flow.", "WELL DONE!");
                timer2.Start();
            }

            if ((gauges[0].getPosition() > 110 || (gauges[0].getPosition() > 65 && direction == -1)) && tutorMode == 1)
            {
                timer2.Stop();
                MessageBox.Show("Notice that the engine did not light off when fuel was toggled on. Furthermore there was no fuel flow, thus it is likely a problem " +
                    "in the fuel delivery system. Release the starter valve and abort the start procedure to avoid engine damage. " +
                    "The problem needs to be adressed immediately by a qualified technician. " +
                    "", "Tutorial Warning");
                if (fuelOnBlocked == 1)
                {
                    blockedFuel();
                }
                if (startButtonOn == 1)
                {
                    changeStartValve();
                }
                timer2.Start();
            }
        }

        public void blockedFuel()
        {
            if (fuelOnBlocked == -1)
            {
                centerToggle((panel1.Height / 4));
            }
            else
            {
                centerToggle(0);
            }
            fuelOnBlocked = fuelOnBlocked * -1;

            // When fuel is toggled on some gauge changes take place
            if (!mode.Equals("No Light Off- without Fuel Flow"))
            {
                if (fuelOnBlocked == 1)
                {
                    gauges[6].setSpeed(5);
                    onState[6] = true;
                    gauges[5].setSpeed(8);
                    onState[5] = true;

                    if (gauges[0].getPosition() <= 2)
                    {
                        while (gauges[5].getPosition() < 42)
                        {
                            gauges[5].move();
                            if (gauges[6].getPosition() < 20)
                            {
                                gauges[6].move();
                            }
                        }
                    }
                }

                //When toggled off make changes as well
                else if (fuelOnBlocked == -1 && gauges[0].getPosition() > 2)
                {
                    System.GC.Collect();

                    gauges[1].setSpeed(.7f);
                    onState[1] = true;

                    gauges[0].setSpeed(1.5f);
                    onState[0] = true;
                    gauges[2].setSpeed(.5f);
                    onState[2] = true;
                    gauges[3].setSpeed(1.3f);
                    onState[3] = true;

                    gauges[6].setSpeed(5);
                    onState[6] = true;
                    gauges[5].setSpeed(8);
                    onState[5] = true;
                    gauges[4].setSpeed(.6f);
                    onState[4] = true;
                }
                else if (gauges[0].getPosition() <= 2)
                {
                    System.GC.Collect();

                    while (gauges[5].getPosition() > 0)
                    {
                        gauges[5].moveBack();
                        if (gauges[6].getPosition() > 0)
                        {
                            gauges[6].moveBack();
                        }
                    }

                    if (gauges[5].getPosition() < 0)
                    {
                        onState[5] = false;
                    }
                    if (gauges[6].getPosition() < 0)
                    {
                        onState[6] = false;
                    }

                    gauges[1].setSpeed(1);
                    onState[1] = false;


                    onState[4] = false;

                    gauges[2].setSpeed(0.5f);
                }
            }
        }

        // The method to handle a no error condition
        public void noError()
        {
            //check possible errors
            starterPast();
            starterRepress();
            earlyFuel();
            earlyFuelFail();

            // if the scenario has ended, reset things.
            stopped();
            avoidHS();

            // move the gauges
            tenPercentMove();

            fuelToggledOnStop();

            //egtMotion();

            oilPressureMotion();

            steadyToIdle();

        }

        // A method to disengage a stuck starter
        public void disengage()
        {
            if(startButtonOn == 1)
            {
                disengaged = true;
                changeStartValve();
                attemptOff = 0;

                if (gauges[0].getPosition() > 65)
                {
                    errorHandled = 3;
                }
                if (fuelOn == 1)
                {
                    changeFuelButton();
                }
            }
        }

        // The random mode
        public void RandomMode()
        {
            int theNum = 10;
            Random randNum = new Random();

            while (theNum > 7)
            {
                theNum = randNum.Next() % 10;
            }

            switch( theNum )
            {
                case 0:
                    mode = "No Oil Pressure";
                    break;
                case 1:
                    mode = "Starter Valve Sticks Open";
                    break;
                case 2:
                    mode = "No Light Off- without Fuel Flow";
                    break;
                case 3:
                    mode = "No Light Off- with Fuel Flow";
                    break;
                case 4:
                    mode = "No N1 Rotation";
                    break;
                case 5:
                    mode = "Hung Start";
                    break;
                case 6:
                    mode = "Hot Start";
                    break;
                case 7:
                    mode = "No Error Conditions";
                    break;
            }
        } 

        public void initializeGauges()
        {
            // Initialize all the guage parameters
            //float position, float speed, float minVal, float maxVal, float minAngle, float maxAngle, PictureBox needle
            gauges[0] = new Gauge(0, 1.5f, 0, 360, 407, 47, n2);    // N2
            gauges[1] = new Gauge(0, 1f, 0, 360, 310, -50, egt);    // EGT
            gauges[2] = new Gauge(0, .5f, 0, 360, 285, -75, oilPressure);    // Oil Pressure
            gauges[3] = new Gauge(0, 1.3f, 0, 360, 403, 43, N1);    // N1
            gauges[4] = new Gauge(0, 1f, 0, 360, 200, -160, oilTemp);    //Oil Temp
            gauges[5] = new Gauge(0, 1f, 0, 360, 464, 104, fuelFlow);    //Fuel Flow
            gauges[6] = new Gauge(1.02f, 1f, 0, 360, 286, -74, pressureRatio); // pressure ratio

            for (int i = 0; i < 7; i++)
            {
                onState[i] = false;
            }
        }


    }



    public class Gauge
    {
        // NOTE: This class does NOT contain all functions. Check the specs for details.

        private float position = 5;        // position (see specs for inititial vals)
        private float speed = 0.5f;         // speed (initially should be 0)
        private float minVal = 0;          // min numerical value on dial
        private float maxVal = 10;         // max numerical value
        private float minAngle = 90;      // angle at which min numerical val appears (must be > maxAngle, so add 360 if not > )
        private float maxAngle = 0;       // angle at which max numerical val appears
        private Image masterImg = null;     // the master image that will be copied and rotated each time
        private PictureBox needle;          // object that holds needle image

        public Gauge(float position, float speed, float minVal, float maxVal, float minAngle, float maxAngle, PictureBox needle)
        {
            // create gauge based on reference to needle picture
            // and value parameters
            this.position = position;
            this.speed = speed;
            this.minVal = minVal;
            this.maxVal = maxVal;
            this.minAngle = minAngle;
            this.maxAngle = maxAngle;
            this.needle = needle;
        }

        public float getMaxAngle()
        {
            return maxAngle;
        }

        public float getMinAngle()
        {
            return minAngle;
        }

        public float getPosition()
        {
            return position;
        }

        public void setPosition(float position)
        {
            this.position = position;
        }

        public void setSpeed(float speed)
        {
            this.speed = speed;
        }

        public void setMinVal(float minVal)
        {
            this.minVal = minVal;
        }

        public void setMaxVal(float maxVal)
        {
            this.maxVal = maxVal;
        }
        public void setMinAngle(float min)
        {
            this.minAngle = min;
        }

        public void setMaxAngle(float max)
        {
            this.maxAngle = max;
        }



        public void move()
        {
            if (masterImg == null)
            {
                masterImg = needle.Image;
            }

            // this really doesn't need the if statement, but the demo version has it
            if (position <= maxVal)
            {
                position += speed;
                
                needle.Image = RotateImage(masterImg, new PointF(367, 352), -minAngle + (position - minVal) / (maxVal - minVal) * (minAngle - maxAngle));
                // PointF(100, 100) is rotation point of needle in image
            }

        }

        public void moveBack()
        {
            if (masterImg == null)
            {
                masterImg = needle.Image;
            }
            
            // this really doesn't need the if statement, but the demo version has it
            if (position >= minVal )
            {
                position -= speed;
                
                needle.Image = RotateImage(masterImg, new PointF(367, 352), -minAngle + (position - minVal) / (maxVal - minVal) * (minAngle - maxAngle));
                // PointF(100, 100) is rotation point of needle in image
            }

        }

        // THIS METHOD IS FROM http://www.codeproject.com/Articles/58815/C-Image-PictureBox-Rotations
        public static Bitmap RotateImage(Image image, PointF offset, float angle)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            //create a new empty bitmap to hold rotated image
            Bitmap rotatedBmp = new Bitmap(image.Width, image.Height);
            rotatedBmp.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //make a graphics object from the empty bitmap
            Graphics g = Graphics.FromImage(rotatedBmp);

            //Put the rotation point in the center of the image
            g.TranslateTransform(offset.X, offset.Y);

            //rotate the image
            g.RotateTransform(angle);

            //move the image back
            g.TranslateTransform(-offset.X, -offset.Y);

            //draw passed in image onto graphics object
            g.DrawImage(image, new PointF(0, 0));

            return rotatedBmp;
        }

    }
}
