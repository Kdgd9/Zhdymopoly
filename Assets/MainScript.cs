using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Diagnostics.Tracing;
using TMPro;
using System.Security.Cryptography;
//using UnityEngine.SocialPlatforms.GameCenter;

public class MainScript : MonoBehaviour
{
    public bool NeedCameraCreation = true;

    public Sprite FieldSprite, TrainStationSprite;
    public TMP_FontAsset GameFont;
    public float FontSize;

    Image FieldImage;
    public float FieldScale = 0.1f, FieldGap = 110f;
    public Vector3 BoardCenter = new (0, 0, 0);

    public static List<string> FieldName = new List<string>();
    public static string[] FN_KRASNOYARSK = {
        "СТАРТ", "АРТЕМОВСК" , "Сокровище\nСибири",
        "ЗАОЗЕРНЫЙ" , "Налог на добычу\nполезных ископаемых" , "АЧИНСК\nACS" , "УЯР" , "Шанс",
        "КОДИНСК" , "УЖУР" , "Заграница\n/просто посмотреть" , "БОРОДИНО" , "КРАСЭНЕРГОСБЫТ" , "ИЛАНСКИЙ" , "ЕНИСЕЙСК" ,
        "НОРИЛЬСК\nHAK" , "БОГОТОЛ" , "Сокровище\nСибири" , "ДУДИНКА" , "ДИВНОГОРСК" , "Адлер" , "ШАРЫПОВО" ,
        "Шанс" , "СОСНОВОБОРСК" , "НАЗАРОВО" , "ЧЕРЕМШАНКА\nKCY" , "ЗЕЛЕНОГОРСК" , "ЛЕСОСИБИРСК" , "КРАСКОМ" ,
        "МИНУСИНСК" , "Отправляйся\nза границу", "ЖЕЛЕЗНОГОРСК" , "КАНСК" , "Богатство\nСибири" , "АЧИНСК" , "ЕМЕЛЬЯНОВО\nKJA" ,
        "Шанс" , "НОРИЛЬСК" , "Экологический\nналог" , "КРАСНОЯРСК"};
    public static string[] FN_LONDON = {
        "START", "OLD KENT\nROAD", "Community\nchest", "WHITECHAPEL\nROAD", "income\ntax",
        "KINGS CROSS\nSTATION", "THE ANGEL\nISLINGTON", "chance", "EUSTON ROAD", "PENTONVILLE\nROAD",
        "JUST VISITING\n/ IN JAIL", "PALL MALL", "ELECTRIC\nCOMPANY", "WHITEHALL", "NORTHUMR’D\nAVENUE",
        "MARYLEBONE\nSTATION", "BOW STREET", "community\nchest", "MARLBOROUGH\nSTREET", "VINE STREET",
        "FREE PARKING", "STRAND", "chance", "FLEET STREET", "TRAFALGAR\nSQUARE", "FENCHURCH\nSTATION",
        "LEICESTER\nSQUARE", "COVENTRY\nSTREET", "WATER\nWORKS", "PICCADILLY", "GO TO JAIL", "REGENT STREET",
        "OXFORD STREET", "community\nchest", "BOND STREET", "LIVERPOOL ST.\nSTATION", "chance", "PARK LANE", "super tax", "MAYFAIR" };
    public static int[] FieldPrice = 
        { 0, 60, 0, 60, 0, 200, 
        100, 0, 100, 120, 0, 
        140, 150, 140, 160, 200, 
        180, 0, 180, 200, 0, 
        220, 0, 220, 240, 200, 
        260, 260, 150, 280, 0, 
        300, 300, 0, 320, 200, 
        0, 350, 0, 400 };

    void Start()
    {
        if (NeedCameraCreation == true) 
        {
            CameraCreation();
            GameInitialization();
            gameObject.GetComponent<PlayerSystem>().PlayersInitialization();
        }
        else  
        {
            GameInitialization();
            gameObject.GetComponent<PlayerSystem>().PlayersInitialization();
        }
        //test
    }

    void GameInitialization()
    {
        //TODO
        BoardCenter = new(0, Display.main.renderingHeight * 0.4f, 0);
        //
        Vector3 startFieldPosition = new
            (BoardCenter.x + Display.main.renderingWidth / 2,
            BoardCenter.y - Display.main.renderingHeight / 2, 0);

        FieldScale = Display.main.renderingWidth * 0.0001f;
        FieldGap = Display.main.renderingWidth * 0.1f;

        CanvasCreation();
        BoardCreation(startFieldPosition);
    }    

    void CameraCreation()
    {
        GameObject NewCamera = new("Main Camera");
        NewCamera.transform.position = new Vector3(0, 0, -10);
        NewCamera.transform.localScale = new Vector3(1, 1, 1);
        NewCamera.AddComponent<Camera>();
        NewCamera.GetComponent<Camera>().orthographic = true;
        NewCamera.GetComponent<Camera>().farClipPlane = 1000;
        NewCamera.GetComponent<Camera>().nearClipPlane = -1000;
        NewCamera.GetComponent<Camera>().orthographicSize = Display.main.systemWidth;
    }
    void CanvasCreation()
    {
        GameObject MainboardCanvas = GameObject.Find("MainboardCanvas");
        if (MainboardCanvas != null)
        {
            CreateGameBoardFields();
        }
        else
        {
            CreateMainBoardCanvas();
            CreateGameBoardFields();
        }

        void CreateGameBoardFields()
        {
            GameObject GameBoardFieldsCanvas = new("GameBoardFields");
            GameBoardFieldsCanvas.transform.position = new Vector3(0, 0, 0);
            GameBoardFieldsCanvas.transform.SetParent(MainboardCanvas.transform);
        }
        void CreateMainBoardCanvas()
        {
            MainboardCanvas = new("MainboardCanvas");
            MainboardCanvas.transform.position = new Vector3(0, 0, 0);
            MainboardCanvas.AddComponent<Canvas>();
            MainboardCanvas.AddComponent<GraphicRaycaster>();
            MainboardCanvas.AddComponent<CanvasScaler>();
            MainboardCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            if (GameObject.Find("Main Camera").GetComponent<Camera>() != null)
            {
                MainboardCanvas.GetComponent<Canvas>().worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            }
            else
            {
                MainboardCanvas.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            }
        }
    }
    void BoardCreation(Vector3 startFieldPosition)
    {
        Vector3 newFieldPoition = startFieldPosition;

        FieldCreation();
        KickResultGraphicsCreation();


        void FieldCreation()
        {
            for (int i = 0; i < 40; i++)
            {
                switch (i)
                {
                    case < 10:
                        FieldGameobjectCreation(i, newFieldPoition.x, newFieldPoition.y);
                        newFieldPoition.x -= FieldGap;
                        break;

                    case < 20:
                        FieldGameobjectCreation(i, newFieldPoition.x, newFieldPoition.y);
                        newFieldPoition.y += FieldGap;
                        break;

                    case < 30:
                        FieldGameobjectCreation(i, newFieldPoition.x, newFieldPoition.y);
                        newFieldPoition.x += FieldGap;
                        break;

                    case < 40:
                        FieldGameobjectCreation(i, newFieldPoition.x, newFieldPoition.y);
                        newFieldPoition.y -= FieldGap;
                        break;
                }
            }
        }
        void KickResultGraphicsCreation()
        {
            GameObject CenterDown = GameObject.Find("GameBoardFields/GameField 5");
            GameObject CenterLeft = GameObject.Find("GameBoardFields/GameField 15");
            float KickResultSize = Display.main.renderingHeight / 4;

            GameObject KickResultText = new ("Kick Result");
            KickResultText.AddComponent<TextMeshPro>();
            KickResultText.GetComponent<TextMeshPro>().fontSize = KickResultSize;
            KickResultText.GetComponent<TextMeshPro>().color = Color.black;
            KickResultText.GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.Center;
            KickResultText.GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.Midline;
            KickResultText.GetComponent<TextMeshPro>().fontStyle = FontStyles.Bold;
            KickResultText.GetComponent<TextMeshPro>().font = GameFont;
            KickResultText.transform.position = new Vector3
                (CenterDown.transform.position.x,
                CenterLeft.transform.position.y - Display.main.renderingHeight/8, 0);
            KickResultText.GetComponent<TextMeshPro>().rectTransform.sizeDelta = new Vector2(KickResultSize, KickResultSize / 2);
            KickResultText.transform.SetParent(GameObject.Find($"MainboardCanvas").transform);

        }
    }
    void FieldGameobjectCreation(int n, float LEFT, float TOP)
    {
        FieldName.Add(FN_KRASNOYARSK[n]);
        GameObject block = new($"GameField {n}");
        FieldBlockCreation();
        FieldTextCreation();
        FieldColorBankCreation();

        void FieldBlockCreation()
        {
            block.transform.SetParent(GameObject.Find("GameBoardFields").transform);
            block.transform.position = new Vector2 { x = LEFT, y = TOP };

            if (n % 10 == 0) { block.transform.localScale = new Vector3(FieldScale * 9, FieldScale * 9, 0.1f); }
            else if (n > 0 && n < 10 || n > 20 && n < 30) { block.transform.localScale = new Vector3(FieldScale * 8, FieldScale * 9, 0.1f); }
            else { block.transform.localScale = new Vector3(FieldScale * 9, FieldScale * 8, 0.1f); }

            block.AddComponent<Button>();
            Button FieldClick = block.GetComponent<Button>();
            FieldClick.onClick.AddListener(OnFieldClick);
            block.AddComponent<Image>();
            block.GetComponent<Image>().sprite = FieldSprite;
            FieldImage = block.GetComponent<Image>();
            FieldClick.targetGraphic = FieldImage;
            block.AddComponent<FieldOwner>();
            block.GetComponent<FieldOwner>();
            FieldOwner.Owner = "Null";
        }
        void FieldColorBankCreation()
        {
            GameObject colorbank = new($"Field {n} color bank");
            colorbank.transform.SetParent(GameObject.Find($"GameBoardFields/GameField {n}").transform);
            colorbank.transform.position = new Vector3(block.transform.position.x, block.transform.position.y, -1f);
            colorbank.transform.localScale = block.transform.localScale;
            colorbank.AddComponent<Image>();
            colorbank.GetComponent<Image>().sprite = FieldSprite;
            colorbank.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(100, 30);
            if (n > 10 && n < 20) { colorbank.GetComponent<Image>().rectTransform.Rotate(new Vector3(0, 0, 1), -90); }
            if (n > 30 && n < 40) { colorbank.GetComponent<Image>().rectTransform.Rotate(new Vector3(0, 0, 1), 90); }
            switch (n)
            {
                case 1:
                case 3:
                    colorbank.GetComponent<Image>().color = new Color32(148, 85, 55, 255);
                    break;
                case 6:
                case 8:
                case 9:
                    colorbank.GetComponent<Image>().color = new Color32(170, 224, 252, 255);
                    break;
                case 5:
                case 15:
                case 25:
                case 35:
                    colorbank.GetComponent<Image>().color = Color.gray;
                    /*var blockOpinionalImage = new GameObject();
                    blockOpinionalImage.name = $"Field {n} Opinional Image";                 
                    blockOpinionalImage.transform.parent = GameObject.Find($"/GameBoardFields/GameField {n}").transform;
                    blockOpinionalImage.transform.position = new Vector3(block.transform.position.x, block.transform.position.y, -1f);
                    blockOpinionalImage.transform.localScale = block.transform.localScale;
                    blockOpinionalImage.AddComponent<Image>();               
                    blockOpinionalImage.GetComponent<Image>().sprite = TrainStationSprite;*/
                    break;
                case 11:
                case 13:
                case 14:
                    colorbank.GetComponent<Image>().color = new Color32(217, 58, 150, 255);
                    break;
                case 16:
                case 18:
                case 19:
                    colorbank.GetComponent<Image>().color = new Color32(247, 148, 29, 255);
                    break;
                case 21:
                case 23:
                case 24:
                    colorbank.GetComponent<Image>().color = new Color32(237, 27, 36, 255);
                    break;
                case 26:
                case 27:
                case 29:
                    colorbank.GetComponent<Image>().color = new Color32(254, 242, 0, 255);
                    break;
                case 31:
                case 32:
                case 34:
                    colorbank.GetComponent<Image>().color = new Color32(31, 178, 90, 255);
                    break;
                case 37:
                case 39:
                    colorbank.GetComponent<Image>().color = new Color32(0, 114, 187, 255);
                    break;
                default:
                    Destroy(colorbank);
                    break;
            }
        }
        void FieldTextCreation()
        {
            Vector3 blockPosition = new(block.transform.position.x, block.transform.position.y, -1f);
            FieldNameCreation();
            if (FieldPrice[n] != 0) { FieldPriceCreation(); }

            void FieldNameCreation()
            {
                GameObject blockText = new($"Field Text Name {n}");
                blockText.AddComponent<TextMeshPro>();
                blockText.GetComponent<TextMeshPro>().text = FieldName[n];
                blockText.GetComponent<TextMeshPro>().fontSize = FontSize;
                blockText.transform.SetParent(GameObject.Find($"GameBoardFields/GameField {n}").transform);
                blockText.GetComponent<TextMeshPro>().color = Color.black;
                blockText.GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.Center;
                if (n % 10 == 0) { blockText.GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.Midline; }
                else { blockText.GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.Top; }
                blockText.GetComponent<TextMeshPro>().fontStyle = FontStyles.Bold;
                blockText.GetComponent<TextMeshPro>().enableAutoSizing = true;
                blockText.GetComponent<TextMeshPro>().fontSizeMax = FontSize;
                blockText.GetComponent<TextMeshPro>().fontSizeMin = FontSize / 2;
                blockText.GetComponent<TextMeshPro>().font = GameFont;
                blockText.transform.position = blockPosition;
                blockText.transform.localScale = new Vector3(1, 1, 0.1f);
                blockText.GetComponent<TextMeshPro>().rectTransform.sizeDelta = new Vector2(100, 100);

                if (n > 10 && n < 20) { blockText.GetComponent<TextMeshPro>().rectTransform.Rotate(new Vector3(0, 0, 1), -90); }
                if (n > 30 && n < 40) { blockText.GetComponent<TextMeshPro>().rectTransform.Rotate(new Vector3(0, 0, 1), 90); }
            }
            void FieldPriceCreation()
            {
                GameObject blockTextPrice = new($"Field Text Price {n}");
                blockTextPrice.AddComponent<TextMeshPro>();
                blockTextPrice.GetComponent<TextMeshPro>().text = $"{FieldPrice[n]}";
                blockTextPrice.GetComponent<TextMeshPro>().fontSize = FontSize;
                blockTextPrice.transform.SetParent(GameObject.Find($"GameBoardFields/GameField {n}").transform);
                blockTextPrice.GetComponent<TextMeshPro>().color = Color.black;
                blockTextPrice.GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.Center;
                blockTextPrice.GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.Bottom;
                blockTextPrice.GetComponent<TextMeshPro>().fontStyle = FontStyles.Bold;
                blockTextPrice.transform.position = blockPosition;
                blockTextPrice.GetComponent<TextMeshPro>().font = GameFont;
                blockTextPrice.transform.localScale = new Vector3(1, 1, 0.1f);
                blockTextPrice.GetComponent<TextMeshPro>().rectTransform.sizeDelta = new Vector2(100, 100);

                if (n > 10 && n < 20) { blockTextPrice.GetComponent<TextMeshPro>().rectTransform.Rotate(new Vector3(0, 0, 1), -90); }
                if (n > 30 && n < 40) { blockTextPrice.GetComponent<TextMeshPro>().rectTransform.Rotate(new Vector3(0, 0, 1), 90); }
            }
        }
        void OnFieldClick()
        {
            print($"You touch the {FieldName[n]} field");
        }
    }
}

