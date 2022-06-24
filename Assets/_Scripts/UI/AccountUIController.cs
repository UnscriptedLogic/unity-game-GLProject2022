using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Core;
using Backend;
using System.Threading.Tasks;

namespace UI
{
    public class AccountUIController : MonoBehaviour
    {
        [SerializeField] private MainScreenUIController mainScreenUIController;

        [Header("Sign Up")]
        [SerializeField] private TMP_InputField suUsernameField;
        [SerializeField] private TMP_InputField suEmailField;
        [SerializeField] private TMP_InputField suPasswordField;
        [SerializeField] private TMP_InputField suRepeatPasswordField;
        [SerializeField] private TextMeshProUGUI suErrorTMP;
        [SerializeField] private Button suSubmit;
        
        [Header("Log In")]
        [SerializeField] private TMP_InputField liUsernameField;
        [SerializeField] private TMP_InputField liEmailField;
        [SerializeField] private TMP_InputField liPasswordField;
        [SerializeField] private TextMeshProUGUI liErrorTMP;
        [SerializeField] private Button liSubmit;

        [Header("Log Out")]
        [SerializeField] private Button logoutSubmit;

        [Header("Player Stats")]
        [SerializeField] private TMP_InputField usernameTitle;
        [SerializeField] private TextMeshProUGUI winsTMP;
        [SerializeField] private TextMeshProUGUI lossesTMP;
        [SerializeField] private TextMeshProUGUI highestWaveTMP;
        [SerializeField] private TextMeshProUGUI gamesPlayedTMP;
        [SerializeField] private Transform statLoadingPanel;

        [Header("Username prompt")]
        [SerializeField] private Transform promptPage;
        [SerializeField] private Image promptPanel;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TextMeshProUGUI feedbackText;

        private string[] tabooKeywords = new string[]
        {
            "mum",
            "mom",
            "ur mom",
            "ur dad",
            "ur mum",
            "your mom",
            "your mum",
            "fuck",
            "shit",
            "sht",
            "bitch",
            "bij",
            "bj",
            "btch",
            "cock",
            "dick",
            "dicc",
            "fck",
            "fuk",
            "fk",
            "balls",
            "penis",
            "vagina",
            "pussy",
            "puss",
            "ochinchin",
            "chinchin",
            "oppai",
            "slut",
            "whore",
            "kenja",
            "post nut",
            "daddy"
        };

        private string[] profanityFoundResponse = new string[]
        {
            "Stop it",
            "What were you thinking?",
            "Can you not?",
            "Really? No I'm not allowing that.",
            "Did you really just type that in",
            "Nice try kid",
            "No",
            "Nope",
            "This ain't it chief",
            "Can we be civilized here please",
            "C'mon man. Really?",
            "No profanity/vulgar words please",
            "You tried. A for effort"
        };

        private void Start()
        {
            suErrorTMP.text = "";
            liErrorTMP.text = "";
        }

        public void UpdateUserStats()
        {
            usernameTitle.text = GameManager.username;
            winsTMP.text = $"Wins: {GameManager.wins}";
            lossesTMP.text = $"Losses: {GameManager.loss}";
            highestWaveTMP.text = $"Highest Wave: {GameManager.highestWave}";
            gamesPlayedTMP.text = $"Games Played: {GameManager.gamesPlayed}";
        }

        public void ShowStats()
        {
            statLoadingPanel.gameObject.SetActive(true);
            PlayFabManager.GetPlayerData(res => { statLoadingPanel.gameObject.SetActive(false); }, PlayFabManager.HandleError);

            UpdateUserStats();
        }

        public void SendUsername()
        {
            feedbackText.gameObject.SetActive(false);
            UpdateUsername(inputField.text);
        }

        public void UpdateUsername(string text)
        {
            if (ClearedRestrictions(text))
            {
                PlayFabManager.UpdateUsername(text, res =>
                {
                    mainScreenUIController.HideUsernamePrompt();
                    GameManager.username = text;
                }, PlayFabManager.HandleError);
            }
        }

        public void CheckUsername()
        {
            if (GameManager.username == null)
            {
                mainScreenUIController.ShowUsernamePrompt();
            }
        }

        private bool ClearedRestrictions(string inputText)
        {
            if (inputText == "")
            {
                Notify("- Name cannot be empty! What am I supposed to call you? ''?");
                return false;
            }

            if (inputText.Length < 5)
            {
                Notify("- More than 5 characters please!");
                return false;
            }

            string text = inputText;
            text = text.ToLower();
            text = text.Replace(" ", "");
            text = RemoveSpecialCharacters(text);
            for (int i = 0; i < tabooKeywords.Length; i++)
            {
                if (text.Contains(tabooKeywords[i]))
                {
                    Notify(MathHelper.RandomFromArray(profanityFoundResponse));
                    return false;
                }

                //if (text.Contains("unscriptedlogic"))
                //{
                //    Notify("Name reserved for developer. Sorry :D");
                //    return;
                //}
            }

            return true;
        }

        public string RemoveSpecialCharacters(string str)
        {
            string sb = "";
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb += c;
                }
            }
            return sb.ToString();
        }

        private async void Notify(string text)
        {
            feedbackText.gameObject.SetActive(true);
            feedbackText.text = text;
            await Task.Delay(5000);
            feedbackText.gameObject.SetActive(false);
        }
    }
}