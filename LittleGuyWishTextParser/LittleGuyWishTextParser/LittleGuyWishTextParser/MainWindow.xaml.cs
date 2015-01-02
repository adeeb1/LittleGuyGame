using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LittleGuyWishTextParser
{
    public partial class MainWindow : Window
    {
        // The prefix that starts every wish
        public const String WISHPREFIX = "me wish";

        // Represents a quote (") character
        public const char QUOTECHAR = (char)34;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Lowercase the wish text
            String wish = txtBoxWish.Text.ToLower();

            GrantWish(wish);
        }

        private bool IsValidWish(String wish)
        {
            // Make sure the wish is at least the length of the prefix
            if (wish.Length < WISHPREFIX.Length)
            {
                MessageBox.Show("Please format da wish in da right format. Da wish always start with: " + QUOTECHAR + WISHPREFIX + QUOTECHAR);

                return false;
            }

            // Get the prefix of the user's wish
            String WishPrefix = wish.Substring(0, WISHPREFIX.Length);

            // Make sure the prefix of the player's wish equals the valid wish prefix
            if (WishPrefix != WISHPREFIX)
            {
                MessageBox.Show("Please format da wish in da right format. Da wish always start with: " + QUOTECHAR + WISHPREFIX + QUOTECHAR);

                return false;
            }

            return true;
        }

        // Gets the target of the wish. The target can be the wisher, another player, or the Big Guy
        private String GetWishTarget(String WishTarget1, String WishTarget2)
        {
            // Check if the wish target is the Big Guy
            if (IsWishTargetBigGuy(WishTarget1, WishTarget2))
            {
                // Concatenate the two wish targets
                // NOTE: In an actual game, we will probably return a number of some sort that refers to the Big Guy
                return (WishTarget1 + " " + WishTarget2);
            }
            else // The wish target is not the Big Guy
            {
                // Return only the first wish target since anyone else will not use two words
                return WishTarget1;
            }
        }

        // Checks if the wish target is the Big Guy
        private bool IsWishTargetBigGuy(String WishTarget1, String WishTarget2)
        {
            return (WishTarget1 == "big" || WishTarget1 == "b") && (WishTarget2 == "guy");
        }

        // NOTE: There will be multiple wish types. Some wishes will have one target, and others will have two targets
        // Ex: One target -> "Me wish dat me had a noodle" -> The second "me" is the only target
        // Ex: Two targets -> "Me wish dat me had all of hacky342's items" -> The second "me" and "hacky342" are the targets
        
        // An idea for differentiating the wish types is:
        //   1) Do a quick scan of the wish, and check how many targets there are. To get the number of targets:
        //      a. Loop through all of the words in the wish
        //      b. Check if each word is a valid player
        //      c. If the word is a valid player, increment the total number of targets in the wish by 1
        //   2) Use a conditional, and run the wish through the corresponding parser

        // Grants a wish
        private void GrantWish(String wish)
        {
            //// Check if the wish is invalid
            //if (IsValidWish() == false)
            //{
            //    // Exit the method. Error messages will have been displayed by the IsValidWish method
            //    return;
            //}

            // Split the wish by space so each word is an array element, and remove any empty entries
            String[] WishArray = wish.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            // Get the wish target

            // Stores the word index at which the target of the wish begins
            int WishTargetIndex = 2;
             
            // Check if an optional filler word, such as "da" or "the," is used in the wish
            if (WishArray[WishTargetIndex] == "da" || WishArray[WishTargetIndex] == "dat" || WishArray[WishTargetIndex] == "the" || WishArray[WishTargetIndex] == "that")
            {
                // Make the wish target start at the next word
                WishTargetIndex += 1;

                // Check if yet another optional filler word is used, and if so, make the wish target start at the next word
                if (WishArray[WishTargetIndex] == "da" || WishArray[WishTargetIndex] == "the")
                    WishTargetIndex += 1;
            }

            // Get the object the wish is targeting
            // Ex: "Me wish dat me had a noodle" -> The second "me" is the target
            String WishTarget = GetWishTarget(WishArray[WishTargetIndex], WishArray[WishTargetIndex + 1]);

            // NOTE: The code below would be used in an actual game
            //// Check if the wish target is the wisher
            //if (WishTarget == "me")
            //{
            //    // Set the wish target to the wisher's username
            //    WishTarget = GetUsername(CurrentPlayer);
            //}
            
            //// Get the player index of the target
            //int PlayerIndex = GetPlayerIndex(WishTarget);

            // Get the wish action verb

            // Stores the word index at which the action verb of the wish begins
            // Ex: Me wish me had a noodle -> "had" is the action verb. Other options could be "stole" and "destroyed"
            int WishActionVerbIndex = (WishTargetIndex + 1);

            // Check if the wish target is the Big Guy
            if (IsWishTargetBigGuy(WishArray[WishTargetIndex], WishArray[WishTargetIndex + 1]) == true)
            {
                // Make the wish action verb start at the next word
                WishActionVerbIndex += 1;
            }

            // Get the wish action verb
            String WishActionVerb = WishArray[WishActionVerbIndex];

            // Get the wish quantity

            // Stores the word index at which the quantity would be specified
            int WishQuantityIndex = (WishActionVerbIndex + 1);

            // Stores the wish quantity
            int WishQuantity;

            // Check if we cannot get the wish quantity
            if (int.TryParse(WishArray[WishQuantityIndex], out WishQuantity) == false)
            {
                // Check if the wish quantity specified is "a," "the," or "da"
                if (WishArray[WishQuantityIndex] == "a" || WishArray[WishQuantityIndex] == "the" || WishArray[WishQuantityIndex] == "da")
                {
                    // Set the quantity to 1
                    WishQuantity = 1;
                }
                else // No wish quantity was specified, or it is in the incorrect format
                {
                    // Decrement the wish quantity index by 1 so that the wish action includes the word that normally would've been
                    // reserved for the quantity
                    WishQuantityIndex -= 1;
                }
            }

            // Get the wish action

            // Stores the word index at which the action of the wish begins
            int WishActionIndex = (WishQuantityIndex + 1);

            // Get the action by combining the rest of the words in the wish. Separate each word by space
            String WishAction = String.Join(" ", WishArray, WishActionIndex, (WishArray.Length - WishActionIndex));

            // Try to grant the wish based on the action specified
            switch (WishAction)
            {
                case "textbox":
                    for (int i = 0; i < WishQuantity; i++)
                    {
                        // Wish for a text box
                        WishForTextBox((WishActionVerb == "had"));
                    }

                    break;
            }
        }

        // Wishes for a text box
        private void WishForTextBox(bool IsWisherReceiving)
        {
            // Check if the wisher is receiving a text box
            if (IsWisherReceiving == true)
            {
                TextBox box = new TextBox();

                ContentGrid.Children.Add(box);
            }
            else // The wisher is losing a text box
            {
                ContentGrid.Children.RemoveAt((ContentGrid.Children.Count - 1));
            }
        }

        private void txtBoxWish_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                Button_Click(btnWish, new RoutedEventArgs());
            }
        }


    }
}