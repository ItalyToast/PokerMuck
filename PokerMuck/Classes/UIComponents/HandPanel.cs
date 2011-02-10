﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace PokerMuck
{
    public partial class HandPanel : UserControl
    {

        /* Holds the current hand to display */
        private Hand handToDisplay;
        public Hand HandToDisplay
        {
            get
            {
                return handToDisplay;
            }

            set
            {
                handToDisplay = value;
                ReloadGraphics();
            }
        }

        [Description("Sets the border padding value (distance from the border)"),
         Category("Values"),
         DefaultValue(4)]
        public int BorderPadding { get; set; }

        [Description("Sets the space between cards"),
         Category("Values"),
         DefaultValue(40)]
        public int CardSpacing { get; set; }

        /* Every reference to a CardPictureBox is stored here */
        private List<CardPictureBox> cardPictures;

       
        public HandPanel()
        {
            InitializeComponent();
            cardPictures = new List<CardPictureBox>(10);
        }

        /* Clears every previous graphic added to the panel,
         * loads the new graphics and displays the new graphics */
        private void ReloadGraphics()
        {
            ClearCardPictures();
            LoadCardPictures();

            // Do we even need to paint anything?
            if (cardPictures.Count > 0)
            {
                ScaleCardPictures();
                MoveCardPictures();
                DisplayCardPictures();
            }
        }

        /* This method will load the card pictures (if available) */
        private void LoadCardPictures()
        {
            if (handToDisplay != null)
            {
                // Generate the card picture boxes for each card in the hand
                foreach (Card c in handToDisplay.Cards)
                {
                    CardPictureBox pictureBox = new CardPictureBox(c);
                    cardPictures.Add(pictureBox);
                }
            }
        }

        /* Takes all the elements in cardPictures and draws them */
        private void DisplayCardPictures()
        {
            foreach (CardPictureBox pictureBox in cardPictures)
            {
                this.Controls.Add(pictureBox);
            }
        }

        private void MoveCardPictures()
        {
            // Where are we drawing the next component?
            int currentX = BorderPadding, currentY = 0;
            int containerHeight = this.Size.Height;
            int i = 0; // counter

            foreach (CardPictureBox pictureBox in cardPictures)
            {
                // Set Y at the center
                currentY = containerHeight / 2 - pictureBox.Height / 2;

                // Move component
                pictureBox.Top = currentY;
                pictureBox.Left = currentX;

                // Increase X
                currentX += CardSpacing + pictureBox.Width;
                
                i++;
            }
        }

        /* Remove all cards and associated components (if any) */
        private void ClearCardPictures()
        {
            if (cardPictures.Count > 0) cardPictures.Clear();
            this.Controls.Clear();
        }

        // Space in the panel is fixed, so depending on how many cards we have we need to resize the pictures, move them, etc.
        private void ScaleCardPictures()
        {
            // Space available to draw in the space (total - padding)
            int widthAvailable = this.ClientSize.Width - BorderPadding * 2;
            int heightAvailable = this.ClientSize.Height - BorderPadding * 2;

            int numCards = cardPictures.Count;
            Debug.Assert(numCards > 0, "Trying to adjust zero cards?");

            // Retrieve the cards original width
            float originalCardWidth = (float)cardPictures[0].Width;
            float originalCardHeight = (float)cardPictures[0].Height;

            /* If we have N cards, we need (N-1) * Spacing space reserved for padding, thus   
             * every card is going to be (spaceAvailable - (N-1) * spacing) / N
            */
            float allowedCardWidth = (widthAvailable - (numCards - 1) * CardSpacing) / numCards;

            /* Height is easier, just take the height available and subtract double spacing */
            float allowedCardHeight = heightAvailable - BorderPadding * 2;

            /* Find scale factor */
            float widthScaleFactor = allowedCardWidth / originalCardWidth;
            float heightScaleFactor = allowedCardHeight / originalCardHeight;

            float scaleFactorValue = Math.Min(widthScaleFactor, heightScaleFactor); // TODO check

            SizeF scaleFactor = new SizeF(scaleFactorValue, scaleFactorValue);

            // Scale each card
            foreach (CardPictureBox cardPicture in cardPictures)
            {
                cardPicture.Scale(scaleFactor);
            }
        }
    }
}
