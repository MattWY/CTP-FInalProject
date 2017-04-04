using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonManager : MonoBehaviour {

    public static ButtonManager button_instance = null;
    PlaceMonument place_instance;

    //BuildingManager building_instance = null;

    public GameObject buttonGO;
    public GameObject buttonMon;
    public GameObject buttonRes;
    public GameObject monument;
    public GameObject church;

    public GameObject buttonGOClear;
    public GameObject rightImage;

    public Slider rightSlider;
    public Slider leftSlider;

    //public ui elements
    public Text seedField;
    public Text seedFiledCheck;

    public Text seedField2;
    public Text seedFiledCheck2;

    public Text turnLeftField;
    public Text turnRightField;

    //pass these over to the turtle
    [SerializeField]
    public string roadSeed;
    public string roadSeed2;

    //our current values for the road turns
    public float currentRightAngle;
    public float currentLeftAngle;

    //button values for the river
    public bool localValue;
    public bool globalValue;

    /// <summary>
    /// the user has pressed next and wishes to see the next UI space
    /// </summary>
    void finishRoadDrawing()
    {
        Button finishedButton = buttonGO.GetComponent<Button>();
        finishedButton.onClick.AddListener(() => {

            Turtle.turtle_instance.stopGenerating = true;
            //buttonMon.SetActive(true);
            //buttonRes.SetActive(true);
            //rightImage.SetActive(true);

            //find the points so we can generate a river
            GameObject.Find("RiverManager").GetComponent<River>().findSmallestPoints();
            GameObject.Find("RiverManager").GetComponent<River>().findLargestPoints();


        });
    }

    /// <summary>
    /// clears all gameobjects from the scene
    /// </summary>
    void clearCurrentScene()
    {
        Button clearRoad = buttonGOClear.GetComponent<Button>();
        clearRoad.onClick.AddListener(() => {

            //functionality that resets params so we can create a new canvas
            Turtle.turtle_instance.stopGenerating = false;
            Turtle.turtle_instance.clearGameObjectList();
            Turtle.turtle_instance.clearCubeList();
            Turtle.turtle_instance.clearColliderList();
            Turtle.turtle_instance.xPos = 0;
            Turtle.turtle_instance.zPos = 0;

            GameObject.Find("RiverManager").GetComponent<River>().resetParams();
            GameObject.Find("RiverManager").GetComponent<River>().hasRiverGenerated = false;

        });
    }

    /// <summary>
    /// find out what the generate river tick box is..
    /// </summary>
    /// <param name="localValue"></param>
    public void GetValue(bool localValue)
    {
        globalValue = localValue;
    }

    /// <summary>
    /// monument addition
    /// </summary>
    public void addCommercial()
    {
        //if the button has been pressed, then add a commercial mon
        Button addMon = buttonMon.GetComponent<Button>();
        addMon.onClick.AddListener(() => { place_instance.placeBuilding(monument);
        });
    }

    /// <summary>
    /// monument addition
    /// </summary>
    public void addResidential()
    {
        //if the button has been pressed, add a residential mon
        Button resMon = buttonRes.GetComponent<Button>();
        resMon.onClick.AddListener(() => { place_instance.placeBuilding(church);
        });
    }

    /// <summary>
    /// function for checking the first seed
    /// </summary>
    /// <returns></returns>
    bool checkRoadSeedValues()
    {
       foreach(char c in roadSeed)
        {
            if(c != 'A' && c != 'B' && c != 'C' && c != 'D' && c != 'E')
            {
                seedFiledCheck.text = "Invalid first seed.";
                return false;
            }
        }
        seedFiledCheck.text = "Valid first seed.";
        return true;
    }

    /// <summary>
    /// function for checking the second seed
    /// </summary>
    /// <returns></returns>
    bool checkRoadSeedValues2()
    {
        foreach (char c in roadSeed2)
        {
            if (c != 'A' && c != 'B' && c != 'C' && c != 'D' && c != 'E')
            {
                seedFiledCheck2.text = "Invalid second seed.";
                return false;
            }
        }
        seedFiledCheck2.text = "Valid second seed.";
        return true;
    }

    /// <summary>
    /// clear the UI space
    /// </summary>
    void clearFirstUI()
    {
        Canvas set = GameObject.Find("ProgramUI").GetComponent<Canvas>();
        set.gameObject.SetActive(false);

    }

    /// <summary>
    /// Check the contents of the seed boxes
    /// </summary>
    private void checkIfValidSeeds()
    {
        //aif both seeds are faulty
        if (!checkRoadSeedValues() || !checkRoadSeedValues2())
        {
            Turtle.turtle_instance.stopGenerating = true;
        }

        //if both of the seeds are correct
        if (checkRoadSeedValues() || checkRoadSeedValues2())
        {
            Turtle.turtle_instance.stopGenerating = false;
        }

        //if one seed is faulty
        if ((!checkRoadSeedValues() && checkRoadSeedValues2()))
        {
            Turtle.turtle_instance.stopGenerating = true;
        }

        //if one seed is faulty
        if (checkRoadSeedValues() && !checkRoadSeedValues2())
        {
            Turtle.turtle_instance.stopGenerating = true;
        }
    }

    /// <summary>
    /// Init
    /// </summary>
    void Start()
    {
        //init some parameters
        place_instance = GetComponent<PlaceMonument>();
        finishRoadDrawing();
        clearCurrentScene();
        addCommercial();
        addResidential();

        roadSeed = "A";
        roadSeed2 = "A";
    }

    void Awake()
    {
        //button singleton
        if (button_instance == null)
        {
            button_instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// This tick provides us with ways of checking update fields, to check for errors with user input
    /// </summary>
    void Update()
    {
        //get both of our road seed
        roadSeed = seedField.text.ToString();
        roadSeed2 = seedField2.text.ToString();

        checkIfValidSeeds();

        //set the slider values
        currentLeftAngle = leftSlider.value;
        currentRightAngle = rightSlider.value;

        //and then the text values
        turnLeftField.text = currentLeftAngle.ToString();
        turnRightField.text = currentRightAngle.ToString();

    }


}
