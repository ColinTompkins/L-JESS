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
        public mainWindow()
        {
            InitializeComponent();
        }

        // Form will load, start the timer that allows the splash and then loads the main screen.
        // Also hide the help panel, since it is actuallty in front
        // And call the center toggle method to put the fuel toggle in the right screen location.
        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
            helpPanel.Hide();
            centerSplashImage();
            centerToggle(0);
            relocateInfoButton();

        }

        // Declare some necessary variables
        int tutorMode = -1;
        int playPause = 1;
        Boolean offBox = false;
        int splitterLoc = 0;
        int fuelOn = -1;
        private EventArgs e;
        int startButtonOn = -1;


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

        private void menuBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            dTitle.ForeColor = Color.White;
            switch (menuBox.SelectedItem.ToString())
            {
                case "Random Error Conditions":
                    dTitle.Text = menuBox.SelectedItem.ToString();
                    description.Text = "Various error conditions will be chosen randomly to "
                        + "better simulate a real starting experience with unknown problems.";
                    break;
                case "No Oil Pressure":
                    dTitle.Text = menuBox.SelectedItem.ToString();
                    description.Text = "This simulates an engine start in the case of no oil pressure.";
                    break;
                case "Starter Valve Sticks Open":
                    dTitle.Text = menuBox.SelectedItem.ToString();
                    description.Text = "This simulates a start up in the case of a sticking starter valve.";
                    break;
                case "No Light Off- without Fuel Flow":
                    dTitle.Text = menuBox.SelectedItem.ToString();
                    description.Text = "This simulates an engine start if there is no fuel flowing and No light off.";
                    break;
                case "No Light Off- with Fuel Flow":
                    dTitle.Text = menuBox.SelectedItem.ToString();
                    description.Text = "This simulates an engine start if there is fuel flowing and No light off.";
                    break;
                case "No N1 Rotation":
                    dTitle.Text = menuBox.SelectedItem.ToString();
                    description.Text = "This simulates an engine start where the N1 is not rotating.";
                    break;
                case "Hung Start":
                    dTitle.Text = menuBox.SelectedItem.ToString();
                    description.Text = "This simulates a start where the engine is hung.";
                    break;
                case "Hot Start ":
                    dTitle.Text = menuBox.SelectedItem.ToString();
                    description.Text = "This simulates an engine start where the engine is still hot from a previous use.";
                    break;
                case "No Error Conditions":
                    dTitle.Text = menuBox.SelectedItem.ToString();
                    description.Text = "This simulates an engine start with no issues. A normal start procedure.";
                    break;

            }
            settingButton.BackColor = Color.Transparent;
            settingHighlight.BackColor = description.BackColor;
        }

        // If the user selects the setting icon, expand the menu and remove highlight
        private void pictureBox1_Click_2(object sender, EventArgs e)
        {
            if (splitContainer1.SplitterDistance == 45)
            {
                menuOut();
            }
            menuBox.Text = "Select a Control Mode";
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
            }
            else
            {
                pauseButton.Image = EngineStartSimulator.Properties.Resources.pause10;
                pauseLabel.Text = "Pause";
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
        }

        private void stopButton_MouseHover(object sender, EventArgs e)
        {
            stopButton.BackColor = Color.DarkGray;
            stopHighlight.BackColor = Color.DarkGray;
            stopLabel.BackColor = Color.DarkGray;
        }

        private void stopButton_MouseLeave(object sender, EventArgs e)
        {
            stopButton.BackColor = Color.Transparent;
            stopHighlight.BackColor = description.BackColor;
            stopLabel.BackColor = Color.Transparent;
        }

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

        }

        private void restartButton_MouseHover(object sender, EventArgs e)
        {
            restartButton.BackColor = Color.DarkGray;
            restartLabel.BackColor = Color.DarkGray;
            restartHighlight.BackColor = Color.DarkGray;
        }

        private void restartButton_MouseLeave(object sender, EventArgs e)
        {
            restartButton.BackColor = Color.Transparent;
            restartLabel.BackColor = Color.Transparent;
            restartHighlight.BackColor = description.BackColor;
        }

        private void restartButton_MouseDown(object sender, MouseEventArgs e)
        {
            restartButton.BackColor = Color.Black;
            restartLabel.BackColor = Color.Black;
            restartHighlight.BackColor = Color.Black;
        }

        private void tutorHighlight_TextChanged(object sender, EventArgs e)
        {

        }

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
            splitContainer1.SplitterDistance = 45;
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

        // Double clicking the mouse will put the mune panel out
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

        private void backgroundBox_DoubleClick(object sender, EventArgs e)
        {
            menuIn();
        }

        // Handle the splash screen time with a timer.
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            splitContainer1.Show();
            menuOut();
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

        // When ,ouse leaves, unhighlight
        private void backButton_MouseLeave(object sender, EventArgs e)
        {
            backButton.BackColor = Color.Transparent;
            backHighlight.BackColor = Color.Transparent;
        }

        // Don't need to do anything here
        private void guagePanel_Paint(object sender, PaintEventArgs e)
        {

        }

        // When the fuel slider button is clicked move it to the correct location.
        private void sliderButton_Click(object sender, EventArgs e)
        {
            changeFuelButton();
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

        // Using the centering method, this mathod handles moving the toggle switch to its new locations.
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
        }

        // When the start valve is clicked, register that in the startButtonOn variable
        // And take care of the animation. 
        // This turns off the valve
        private void startValve_Click(object sender, EventArgs e)
        {
            startValve.Image = EngineStartSimulator.Properties.Resources.start_valveN;
            startButtonOn = startButtonOn * -1;
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
            if (startButtonOn == -1)
            {
                startValve.Image = EngineStartSimulator.Properties.Resources.start_valve_down2;
            }
            else
            {
                startValve.Image = EngineStartSimulator.Properties.Resources.start_valveN;
            }
            startButtonOn = startButtonOn * -1;
        }

        // This turns on the valve
        // while the mouse is held down, the valve stays open
        // register that in the startButtonOn variable.
        private void startValve_MouseDown(object sender, MouseEventArgs e)
        {
            startValve.Image = EngineStartSimulator.Properties.Resources.start_valve_down2;
            startButtonOn = startButtonOn * -1;
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
                    startValve.Image = EngineStartSimulator.Properties.Resources.start_valve_down1;
                    startButtonOn = startButtonOn * -1;
                    break;
                case Keys.Space:
                    changeFuelButton();
                    break;
                case Keys.P:
                    break;
                case Keys.R:
                    break;
                case Keys.S:
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
                    break;
                case Keys.Q:
                    break;
                case Keys.T:
                    break;
            }
     

        }

        private void mainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
            {
                startValve.Image = EngineStartSimulator.Properties.Resources.start_valveN;
                startButtonOn = startButtonOn * -1;
            }
            else
            {
                //e.SuppressKeyPress = true;
            }

        }

    }
}
