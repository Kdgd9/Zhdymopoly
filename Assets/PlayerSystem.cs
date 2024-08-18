using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
//using Unity.VisualScripting;
//using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Events;
using UnityEngine.UI;
//using static UnityEditor.Experimental.GraphView.GraphView;
//using UnityEngine.UIElements;

public class PlayerSystem : MonoBehaviour
{
    public Sprite PlayerChipSprite, ButtonSprite;
    private float PlayerChipSize = 25f, UISize =  200;
    private string MainButtonText;
    public TMP_FontAsset GameFont;
    //private static GameObject playerChip;

    private static int KickResult;
    public string KickStageButtonText = "¡–Œ—»“‹  ”¡» »", EndTurnStageButtonText = "«¿ ŒÕ◊»“‹ ’Œƒ", 
                    PurchaseButtonInputText = " ”œ»“‹ «¿", AnotherKickStageButtonText = "¡–Œ—»“‹ ≈Ÿ≈ –¿«";
    private string KickStage = "KickStage", EndTurnStage = "EndTurnStage"; 
    private List<string> CurrentStage = new();
    private int CurrentPlayerTurn, TotalTurnRound = 0;
    private List<GameObject> PlayerBlockPart = new();

    public int StartMoney = 2000, Salary = 200, SalaryBonus = 100;
    private static List<int> MoneyList = new();
    public string GameCurrency = "$";

    private List<GameObject> Player = new();
    private List<int> PlayerFieldList = new();
    public short NumberOfPlayers = 4;
    public float PlayersGap  = 10;
    public Color[] PlayerColors = { Color.red, Color.blue, Color.green, Color.cyan };
    public string[] PlayerColorsName = { "Red" , "Blue", "Green", "Mint" };

    private static int WindowSize;

    public void PlayersInitialization()
    {
        GameObject CenterDown = GameObject.Find("GameBoardFields/GameField 5");
        GameObject CenterLeft = GameObject.Find("GameBoardFields/GameField 15");
        GameObject LeftDown = GameObject.Find("GameBoardFields/GameField 10");
        GameObject RightDown = GameObject.Find("GameBoardFields/GameField 0");

        WindowSize = Display.main.systemWidth; 
        UISize = WindowSize / 4;
        PlayerChipSize = WindowSize / 6;

        PlayersCreation(CenterDown, CenterLeft, LeftDown, RightDown);
        MainButtonCreation(CenterDown, CenterLeft, LeftDown, RightDown);
    }

    void PlayerListUpdate()
    {
        string PlayerColor;
        switch (CurrentPlayerTurn) 
        {
            default:
                PlayerColor = PlayerColorsName[CurrentPlayerTurn];
                break;
        }

        PlayerBlockPart[CurrentPlayerTurn].GetComponent<TextMeshPro>().text = $"Player {PlayerColor} - {MoneyList[CurrentPlayerTurn]}{GameCurrency} - Field {PlayerFieldList[CurrentPlayerTurn]}";
        CurrentPlayerHighlight();

        void CurrentPlayerHighlight()
        {
            var TempGUI = GameObject.FindGameObjectsWithTag("Highlight");
            for (int i = 0; i < TempGUI.Length; i++)
            {
                Destroy(TempGUI[i]);
            }

            GameObject CurrentPlayer = GameObject.Find($"PlayerBlockPart {CurrentPlayerTurn}");
            GameObject CurrentPlayerHighlight = new("Current Player Highlight");

            CurrentPlayerHighlight.transform.position = new(CurrentPlayer.transform.position.x, CurrentPlayer.transform.position.y);
            CurrentPlayerHighlight.AddComponent<Image>();
            CurrentPlayerHighlight.GetComponent<Image>().sprite = ButtonSprite;
            CurrentPlayerHighlight.GetComponent<Image>().rectTransform.sizeDelta = new(UISize * 2, UISize/8);
            CurrentPlayerHighlight.transform.SetParent(CurrentPlayer.transform);
            CurrentPlayerHighlight.tag = "Highlight";
        }
    }
    void PlayersCreation(GameObject CenterDown, GameObject CenterLeft, GameObject LeftDown, GameObject RightDown )
    {
        GameObject PlayerListBlock = new("PlayerListBlock");
        PlayerListImageCreation();

        for (CurrentPlayerTurn = 0; CurrentPlayerTurn < NumberOfPlayers; CurrentPlayerTurn++)
        {
            MoneyList.Add( new int() );
            MoneyList[CurrentPlayerTurn] = StartMoney;
            CurrentStage.Add(new string(KickStage) );
            Debug.Log($"Player {CurrentPlayerTurn} initialization");

            PlayerChipCreation(CurrentPlayerTurn);
            PlayerListTextCreation(CurrentPlayerTurn);
        }
        //Choosing first player
        CurrentPlayerTurn = UnityEngine.Random.Range(0, NumberOfPlayers);
        PlayerListUpdate();

        void PlayerChipCreation(int i)
        {
            Player.Add(new GameObject($"Player {i} Chip"));
            GameObject StartField = GameObject.Find("GameBoardFields/GameField 0");
            Player[i].transform.position = new Vector3
                (StartField.transform.position.x, 
                StartField.transform.position.y - CurrentPlayerTurn * PlayersGap, 10);
            Player[i].transform.localScale = new Vector3(PlayerChipSize, PlayerChipSize, 0.1f);

            var spriteRenderer = Player[i].AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = PlayerChipSprite;
            spriteRenderer.sortingOrder = 10;
            spriteRenderer.color = PlayerColors[i];

            Player[i].AddComponent<PlayerScript>();
            PlayerFieldList.Add( new() );
        }
        void PlayerListTextCreation(int i)
        {
            PlayerBlockPart.Add(new GameObject($"PlayerBlockPart {i}"));
            PlayerBlockPart[i].AddComponent<TextMeshPro>();
            PlayerBlockPart[i].GetComponent<TextMeshPro>().font = GameFont;
            PlayerBlockPart[i].GetComponent<TextMeshPro>().fontSize = UISize;
            PlayerBlockPart[i].GetComponent<TextMeshPro>().color = Color.black;
            PlayerBlockPart[i].GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.Center;
            PlayerBlockPart[i].GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.Midline;
            PlayerBlockPart[i].GetComponent<TextMeshPro>().fontStyle = FontStyles.Bold;
            //PlayerBlockPart[i].GetComponent<TextMeshPro>().enableAutoSizing = true;
            PlayerBlockPart[i].transform.position =
                new Vector3(PlayerListBlock.transform.position.x, PlayerListBlock.transform.position.y + CurrentPlayerTurn * UISize / 4, -10);
            PlayerBlockPart[i].GetComponent<TextMeshPro>().rectTransform.sizeDelta = new Vector2(UISize * 2, UISize / 4);
            PlayerBlockPart[i].transform.SetParent(PlayerListBlock.transform);
            PlayerListUpdate();
        }
        void PlayerListImageCreation()
        {
            //float PlayerBlockGapModificator = PlayerChipSize * 2.5f;
            Vector3 PlayerListBlockPosition = new
                (CenterDown.transform.position.x,
                CenterLeft.transform.position.y, 0);

            PlayerListBlock.transform.position = PlayerListBlockPosition;
            PlayerListBlock.AddComponent<Image>();
            PlayerListBlock.GetComponent<Image>().sprite = ButtonSprite;
            PlayerListBlock.GetComponent<Image>().rectTransform.sizeDelta =
                new Vector3(LeftDown.transform.position.x + RightDown.transform.position.x, NumberOfPlayers * 20, 0);
            PlayerListBlock.transform.SetParent(GameObject.Find($"MainboardCanvas").transform);
        }
    }
    void MainButtonCreation(GameObject CenterDown, GameObject CenterLeft, GameObject LeftDown, GameObject RightDown)
    {
        GameObject CenterButtonObject;
        GameObject CenterButtonText;
        Vector2 CenterPoint = new Vector2(CenterDown.transform.position.x, CenterDown.transform.position.y + CenterDown.transform.position.y*2);
        MainButtonObjectCreation();
        MainButtonTextCreation();

        void MainButtonObjectCreation()
        {
            CenterButtonObject = new GameObject("Main Button");
            CenterButtonObject.transform.position = CenterPoint;
            CenterButtonObject.AddComponent<RectTransform>();
            CenterButtonObject.GetComponent<RectTransform>().sizeDelta = new Vector2(UISize, UISize / 4);
            CenterButtonObject.AddComponent<Button>();
            CenterButtonObject.AddComponent<Image>();
            CenterButtonObject.GetComponent<Image>().sprite = ButtonSprite;
            CenterButtonObject.transform.SetParent(GameObject.Find($"MainboardCanvas").transform);

            CenterButtonObject.GetComponent<Button>().onClick.AddListener(OnMainButtonPress);
        }
        void MainButtonTextCreation()
        {
            MainButtonText = KickStageButtonText;
            CenterButtonText = new("Main Button Text");
            CenterButtonText.AddComponent<TextMeshPro>();
            CenterButtonText.GetComponent<TextMeshPro>().font = GameFont;
            CenterButtonText.GetComponent<TextMeshPro>().text = MainButtonText;
            CenterButtonText.GetComponent<TextMeshPro>().fontSize = UISize;
            CenterButtonText.GetComponent<TextMeshPro>().color = Color.black;
            CenterButtonText.GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.Center;
            CenterButtonText.GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.Midline;
            CenterButtonText.GetComponent<TextMeshPro>().fontStyle = FontStyles.Bold;
            CenterButtonText.transform.position = new Vector3(CenterPoint.x, CenterPoint.y, -9f);
            CenterButtonText.GetComponent<TextMeshPro>().rectTransform.sizeDelta = new Vector2(UISize * 0.9f, UISize * 0.9f / 4);
            CenterButtonText.transform.SetParent(CenterButtonObject.transform);
        }
    }
    private void DiceKick()
    {
        int FirstDiceKick = UnityEngine.Random.Range(1, 6);
        int SecondDiceKick = UnityEngine.Random.Range(1, 6);
        KickResult = (FirstDiceKick + SecondDiceKick);
        //Debug.Log($"{FirstDiceKick} + {SecondDiceKick} = {KickResult}");

        GameObject KickResultText = GameObject.Find("Kick Result");
        KickResultText.GetComponent<TextMeshPro>().text = $"{FirstDiceKick} {SecondDiceKick}";

        MoveByKick(FirstDiceKick, SecondDiceKick);
    }
    void MoveByKick(int FirstDiceKick, int SecondDiceKick)
    {
        int startFieldIndex = PlayerFieldList[CurrentPlayerTurn];
        int finishFieldIndex = PlayerFieldList[CurrentPlayerTurn] + KickResult;

        if (FirstDiceKick == SecondDiceKick)
        {
            GameObject MainButtonTextObject = GameObject.Find("Main Button Text");
            CurrentStage[CurrentPlayerTurn] = KickStage;
            MainButtonText = AnotherKickStageButtonText;
            MainButtonTextObject.GetComponent<TextMeshPro>().text = MainButtonText;
        }
        if (finishFieldIndex == 40) 
        {
            finishFieldIndex -= 40;
            MoneyList[CurrentPlayerTurn] += Salary + SalaryBonus;
        }
        else if (finishFieldIndex > 39)
        {
            finishFieldIndex -= 40;
            MoneyList[CurrentPlayerTurn] += Salary;
        }
        GameObject finishField = GameObject.Find($"GameBoardFields/GameField {finishFieldIndex}");

        int NumberPlayersOnThatField = 0;
        for (int i  = 0;  i < NumberOfPlayers; i++)
        {
            if (GameObject.Find($"GameBoardFields/GameField {i}") == GameObject.Find($"GameBoardFields/GameField {finishFieldIndex}"))
            {
                NumberPlayersOnThatField++;
            }
        }
        Debug.Log($"NumberPlayersOnThatField: {NumberPlayersOnThatField}");
        Player[CurrentPlayerTurn].transform.position = new (finishField.transform.position.x, finishField.transform.position.y - NumberPlayersOnThatField * PlayersGap, 10);
        if (MainScript.FieldPrice[finishFieldIndex] > 0)
        {
            FieldPurchaseSystem(finishFieldIndex);
        }
        PlayerFieldList[CurrentPlayerTurn] = finishFieldIndex;
    }
    public void OnMainButtonPress()
    {
        GameObject CenterButtonText = GameObject.Find("Main Button Text");

        var TempGUI = GameObject.FindGameObjectsWithTag("TempGUI");
        for (int i = 0; i < TempGUI.Length; i++) 
        {
            Destroy(TempGUI[i]);
        }


        if  (CurrentStage[CurrentPlayerTurn] == KickStage)
        {
            Debug.Log("TEST");
            CurrentStage[CurrentPlayerTurn] = EndTurnStage;
            MainButtonText = EndTurnStageButtonText;
            CenterButtonText.GetComponent<TextMeshPro>().text = MainButtonText;
            DiceKick();
            PlayerListUpdate();
        }
        else if  (CurrentStage[CurrentPlayerTurn] == EndTurnStage)
        {
            MainButtonText = KickStageButtonText;
            CenterButtonText.GetComponent<TextMeshPro>().text = MainButtonText;
            CurrentStage[CurrentPlayerTurn] = KickStage;
            CurrentPlayerTurn++;
            if (CurrentPlayerTurn >= NumberOfPlayers)
            {
                TotalTurnRound++;
                CurrentPlayerTurn = 0;
                Debug.Log($"This is Round {TotalTurnRound}");
            }
            PlayerListUpdate();
            Debug.Log($"Now Turn of Player {CurrentPlayerTurn}");
        }
    }
    void FieldPurchaseSystem(int PuchasableField)
    {
        GameObject CenterDown = GameObject.Find("GameBoardFields/GameField 5");
        GameObject CenterLeft = GameObject.Find("GameBoardFields/GameField 15");
        Vector2 ButtonPosition = new Vector2(CenterDown.transform.position.x, CenterLeft.transform.position.y + UISize * 1.25f);
        GameObject PurchaseButtonObject, PurchaseButtonTextObject;

        PurchaseButtonObjectCreation();
        PurchaseButtonTextObjectCration();
        if (MoneyList[CurrentPlayerTurn] > MainScript.FieldPrice[PuchasableField])
        {
            PurchaseButtonObject.GetComponent<Button>().onClick.AddListener(FieldPurchase);
        }
        else
        {
            PurchaseButtonObject.GetComponent<Image>().color= Color.grey;
            PurchaseButtonObject.GetComponent<Button>().interactable = false;
        }

        void PurchaseButtonObjectCreation()
        {
            PurchaseButtonObject = new("Purchase Button");
            PurchaseButtonObject.transform.position = ButtonPosition;
            PurchaseButtonObject.AddComponent<RectTransform>();
            PurchaseButtonObject.GetComponent<RectTransform>().sizeDelta = new Vector2(UISize, UISize / 4);
            PurchaseButtonObject.AddComponent<Button>();
            PurchaseButtonObject.AddComponent<Image>();
            PurchaseButtonObject.GetComponent<Image>().sprite = ButtonSprite;
            PurchaseButtonObject.transform.SetParent(GameObject.Find($"MainboardCanvas").transform);
            PurchaseButtonObject.tag = "TempGUI";
        }
        void PurchaseButtonTextObjectCration()
        {
            PurchaseButtonTextObject = new GameObject("Purchase Button Text");
            PurchaseButtonTextObject.AddComponent<TextMeshPro>();
            PurchaseButtonTextObject.GetComponent<TextMeshPro>().font = GameFont;
            PurchaseButtonTextObject.GetComponent<TextMeshPro>().text = 
                $"{PurchaseButtonInputText} {MainScript.FieldPrice[PuchasableField]}{GameCurrency}" ;
            PurchaseButtonTextObject.GetComponent<TextMeshPro>().fontSize = UISize;
            PurchaseButtonTextObject.GetComponent<TextMeshPro>().color = Color.black;
            PurchaseButtonTextObject.GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.Center;
            PurchaseButtonTextObject.GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.Midline;
            PurchaseButtonTextObject.GetComponent<TextMeshPro>().fontStyle = FontStyles.Bold;
            PurchaseButtonTextObject.transform.position = new (ButtonPosition.x, ButtonPosition.y, -9f);
            PurchaseButtonTextObject.GetComponent<TextMeshPro>().rectTransform.sizeDelta = new (UISize * 0.9f, UISize * 0.9f / 4);
            PurchaseButtonTextObject.transform.SetParent(PurchaseButtonObject.transform);
            PurchaseButtonTextObject.tag = "TempGUI";
        }
        void FieldPurchase()
        {
            MoneyList[CurrentPlayerTurn] -= MainScript.FieldPrice[PuchasableField];
            Debug.Log($"Field {MainScript.FieldName[PuchasableField]} is sucsesfully purchased");
            Destroy(PurchaseButtonTextObject);
            Destroy(PurchaseButtonObject);
            PlayerListUpdate();
        }
    }
}
