using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

/*Alright I need to add a few things.
    1. Functional recent file selector
    2. Add algorithm improvements such as randomizing the order and redoing previous problems.
       Maybe i could do this by reordering the list and decrementing the pointer.
    3. Add a results summary view at the end.
*/

namespace StudyPal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<String, String> parsed_file;
        List<String> terms;
        List<String> defs;
        int current_term = 0;
        List<Button> choiceButtons = new List<Button>();
        Random rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
            ResultLabel.Visibility = Visibility.Collapsed;
            btnContinue.Visibility = Visibility.Collapsed;
            MainMenu.Visibility = Visibility.Visible;
            QuestionView.Visibility = Visibility.Collapsed;
            SetComplete.Visibility = Visibility.Collapsed;
            recentFileSelector.SelectedIndex = 0;

            choiceButtons.Add(btn_ChoiceA);
            choiceButtons.Add(btn_ChoiceB);
            choiceButtons.Add(btn_ChoiceC);
            choiceButtons.Add(btn_ChoiceD);
        }

        #region UI Functions
        private void navigateToHome()
        {
            ResultLabel.Visibility = Visibility.Collapsed;
            btnContinue.Visibility = Visibility.Collapsed;
            MainMenu.Visibility = Visibility.Visible;
            QuestionView.Visibility = Visibility.Collapsed;
            SetComplete.Visibility = Visibility.Collapsed;
            recentFileSelector.SelectedIndex = 0;
        }

        private void navigateToQuestion()
        {
            foreach (var button in choiceButtons)
            {
                button.IsEnabled = true;
                button.BorderBrush = new SolidColorBrush(Color.FromRgb(0x90, 0x90, 0x90));
                GetTxtBl(button).Foreground = Brushes.Black;
            }

            ResultLabel.Visibility = Visibility.Collapsed;
            btnContinue.Visibility = Visibility.Collapsed;
            QuestionView.Visibility = Visibility.Visible;
            MainMenu.Visibility = Visibility.Collapsed;
            SetComplete.Visibility = Visibility.Collapsed;
        }
       
        private void navigateToComplete()
        {
            ResultLabel.Visibility = Visibility.Collapsed;
            btnContinue.Visibility = Visibility.Collapsed;
            MainMenu.Visibility = Visibility.Collapsed;
            QuestionView.Visibility = Visibility.Collapsed;
            SetComplete.Visibility = Visibility.Visible;
        }
        #endregion

        #region Functions

        private static Dictionary<String, String> parse_file(string filePath)
        {
            Dictionary<String, String> parsed = new Dictionary<String, String>();
            try
            {
                // Read all lines from the file
                string[] lines = File.ReadAllLines(filePath);
                string[] result;

                // Display the contents of the file
                foreach (var line in lines)
                {
                    result = line.Split(',', 2);

                    parsed[result[0]] = result[1];
                }
            }
            catch (IOException e)
            {
                Console.WriteLine($"An error occurred while reading the file: {e.Message}");
            }

            return parsed;
        }

        private static List<String> make_terms(Dictionary<String, String> quizDict)
        {
            List<String> t_terms = new List<String>();

            foreach (var kvp in quizDict)
            {
                t_terms.Add(kvp.Key);
            }

            return t_terms;
        }

        private static List<String> make_defs(Dictionary<String, String> quizDict)
        {
            List<String> t_defs = new List<String>();

            foreach (var kvp in quizDict)
            {
                t_defs.Add(kvp.Value);
            }

            return t_defs;
        }

        private List<String> generate_question(String question, Dictionary<String, String> allDefs, List<String> defs)
        {
            List<String> temp_defs = new List<String> (defs);
            List<String> incorrect_answers = new List<String>();

            temp_defs.Remove(allDefs[question]);

            for (int i = 0; i < 3; i++)
            {
                int item = rnd.Next(0, temp_defs.Count);
                incorrect_answers.Add(temp_defs[item]);
                temp_defs.RemoveAt(item);
            }

            incorrect_answers.Add(allDefs[question]);
            // Shuffle the incorrect answers
            incorrect_answers = incorrect_answers.OrderBy(a => rnd.Next()).ToList();

            return incorrect_answers;
        }

        /*
        public static void AdjustFontSizeToFit(TextBlock textBlock)
        {
            textBlock.FontSize = 20;
            // Calculate the desired width and height based on the text block's current size
            double neededWidth = textBlock.ActualWidth;
            double neededHeight = textBlock.ActualHeight;

            Debug.WriteLine(neededWidth);
            Debug.WriteLine(neededHeight);
            Debug.WriteLine(textBlock.RenderSize.Width);
            Debug.WriteLine(textBlock.RenderSize.Height);
            // Create a formatted text object to measure the text size

            // Decrease font size until text fits within available space
            while (neededWidth > textBlock.RenderSize.Width || neededHeight > textBlock.RenderSize.Height)
            {   
                Debug.WriteLine("chat we resize");
                textBlock.FontSize -= 1; // Decrease font size by 1 unit

                neededWidth = textBlock.ActualWidth;
                neededHeight = textBlock.ActualHeight;
            }
        }
        */

        private static void SetButtonContent(Button button, object content)
        {
            TextBlock textBlock = button.Content as TextBlock;
            textBlock.Text = content.ToString();
        }

        private static TextBlock GetTxtBl(Button button)
        {
            TextBlock textBlock = button.Content as TextBlock;
            return textBlock;
        }

        private void display_question(String question, List<String> answers)
        {
            txtbl_Question.Text = question;
            SetButtonContent(btn_ChoiceA, answers[0]);
            SetButtonContent(btn_ChoiceB, answers[1]);
            SetButtonContent(btn_ChoiceC, answers[2]);
            SetButtonContent(btn_ChoiceD, answers[3]);
        }

        private static void displayError(string error)
        {
            ErrorWindow errorWindow = new ErrorWindow();
            errorWindow.errorText.Text = error;
            errorWindow.ShowDialog();
        }

        private void newQuestion(int question_num)
        {
            if (parsed_file != null)
            {
                string question = terms[question_num];
                List<String> question_answers = generate_question(question, parsed_file, defs);
                display_question(question, question_answers);
            } else
            {
                displayError("Please open a file!");
            }
        }

        private void userResponse(int question_num, Button button)
        {
            string response = GetTxtBl(button).Text;

            foreach (var cbutton in choiceButtons)
            {
                cbutton.IsEnabled = false;
                cbutton.BorderBrush = Brushes.LightGray;
                GetTxtBl(cbutton).Foreground = Brushes.LightGray;
            }

            if (parsed_file != null) 
            {
                string t_answer = new string(parsed_file[terms[question_num]]);
                if (response == t_answer)
                {
                    Debug.WriteLine("Yay, you got it right!");
                    ResultLabel.Content = "Correct!";
                    ResultLabel.Foreground = Brushes.LimeGreen;
                    ResultLabel.Visibility = Visibility.Visible;
                    btnContinue.Visibility = Visibility.Visible;
                    button.BorderBrush = Brushes.LimeGreen;
                    GetTxtBl(button).Foreground= Brushes.LimeGreen;
                    return;
                }
                else
                {
                    Debug.WriteLine($"Incorrect, the correct answer is {t_answer}");
                    ResultLabel.Content = "Sorry, Incorrect.";
                    ResultLabel.Foreground = Brushes.Red;
                    ResultLabel.Visibility = Visibility.Visible;
                    btnContinue.Visibility = Visibility.Visible;

                    foreach (var cbutton in choiceButtons)
                    {
                        if (GetTxtBl(cbutton).Text == t_answer)
                        {
                            cbutton.BorderBrush = Brushes.LimeGreen;
                            GetTxtBl(cbutton).Foreground = Brushes.LimeGreen;
                        } else if (GetTxtBl(cbutton).Text == response)
                        {
                            cbutton.BorderBrush = Brushes.Red;
                            GetTxtBl(cbutton).Foreground = Brushes.Red;
                        }
                    }

                    return;
                }
            } else
            {
                displayError("Please open a file!");
            }
        }

        private void beginSet()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|Quiz Files (*.qz)|*.qz|All Files (*.*)|*.*"; // Set the file filter if needed

            bool? result = openFileDialog.ShowDialog();
            string selectedFileName;

            if (result == true)
            {
                selectedFileName = openFileDialog.FileName;
                parsed_file = parse_file(selectedFileName);
                terms = make_terms(parsed_file);
                defs = make_defs(parsed_file);

                foreach (var kvp in parsed_file)
                {
                    Debug.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value}");
                }

                newQuestion(current_term);
            }
            else
            {
                MessageBox.Show("No file selected", "File Selection Canceled");
            }
        }

        #endregion Functions

        #region UI
        private void btn_ChoiceA_Click(object sender, RoutedEventArgs e)
        {
            userResponse(current_term, btn_ChoiceA);
        }

        private void btn_ChoiceB_Click(object sender, RoutedEventArgs e)
        {
            userResponse(current_term, btn_ChoiceB);
        }

        private void btn_ChoiceC_Click(object sender, RoutedEventArgs e)
        {
            userResponse(current_term, btn_ChoiceC);
        }

        private void btn_ChoiceD_Click(object sender, RoutedEventArgs e)
        {
            userResponse(current_term, btn_ChoiceD);
        }

        private void FileNew_Click(object sender, RoutedEventArgs e)
        {
            UnavailableWindow unavailablWindow = new UnavailableWindow();
            unavailablWindow.ShowDialog();
        }

        private void FileOpen_Click(object sender, RoutedEventArgs e)
        {
            beginSet();
            navigateToQuestion();
        }

        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void HelpAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }

        private void recentFileSelector_DropDownClosed(object sender, EventArgs e)
        {
            // Check if no item is selected
            if (recentFileSelector.SelectedIndex == -1)
            {
                // Select the default item to make it visible
                recentFileSelector.SelectedIndex = 0;
            }
        }
        private void mmOpenSet_Click(object sender, RoutedEventArgs e)
        {
            beginSet();
            navigateToQuestion();
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            current_term++;

            foreach (var button in choiceButtons)
            {
                button.IsEnabled = true;
                button.BorderBrush = new SolidColorBrush(Color.FromRgb(0x90, 0x90, 0x90));
                GetTxtBl(button).Foreground = Brushes.Black;
            }

            if (current_term == parsed_file.Count)
            {
                navigateToComplete();
                foreach (var button in choiceButtons)
                {
                    button.BorderBrush = new SolidColorBrush(Color.FromRgb(0x90, 0x90, 0x90));
                    GetTxtBl(button).Foreground = Brushes.Black;
                }
                current_term = 0;
                return;
            }
            newQuestion(current_term);
            ResultLabel.Visibility = Visibility.Collapsed;
            btnContinue.Visibility = Visibility.Collapsed;
        }

        private void mmCreateSet_CLick(object sender, RoutedEventArgs e)
        {
            UnavailableWindow unavailablWindow = new UnavailableWindow();
            unavailablWindow.ShowDialog();
        }

        private void FileHome_Click(object sender, RoutedEventArgs e)
        {
            navigateToHome();
        }

        private void btnSetCompleteRedo_Click(object sender, RoutedEventArgs e)
        {
            current_term = 0;
            navigateToQuestion();
            newQuestion(current_term);
        }

        private void btnSetCompleteHome_Click(object sender, RoutedEventArgs e)
        {
            navigateToHome();
        }

        private void mmOpenRecent_Click(object sender, RoutedEventArgs e)
        {
            UnavailableWindow unavailablWindow = new UnavailableWindow();
            unavailablWindow.ShowDialog();
        }

        #endregion UI
    }
}