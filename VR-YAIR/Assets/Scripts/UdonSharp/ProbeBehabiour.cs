using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ProbeBehabiour : UdonSharpBehaviour
{

    public float VarCount = 0f;
    public string First = "";
    public string Last = "";

    [SerializeField] private bool upTouched;
    public bool UpTouched
    {
        get { return upTouched; }
        set 
        {
            upTouched = value;
            if(value) VarCount++;
            if (VarCount == 1) First = "up";
            if (VarCount >= 4)
            {
                Last = "up";
                CalculateDir();
            }
        }
    }
    [SerializeField] private bool downTouched;
    public bool DownTouched
    {
        get { return downTouched; }
        set 
        {   
            downTouched = value;
            if (value) VarCount++;
            if (VarCount == 1) First = "down";
            if (VarCount >= 4)
            {
                Last = "down";
                CalculateDir();
            }
        }
    }
    [SerializeField] private bool leftTouched;
    public bool LeftTouched
    {
        get { return leftTouched; }
        set
        {
            leftTouched = value;
            if (value) VarCount++;
            if (VarCount == 1) First = "left";
            if (VarCount >= 4)
            {
                Last = "left";
                CalculateDir();
            }
        }
    }
    [SerializeField] private bool rightTouched;
    public bool RightTouched
    {
        get { return rightTouched; }
        set
        {
            rightTouched = value;
            if (value) VarCount++;
            if (VarCount == 1) First = "right";
            if (VarCount >= 4)
            {
                Last = "right";
                CalculateDir();
            }
        }
    }

    void CalculateDir()
    {
        switch (First)
        {
            case "up":
                break;
            case "down":
                break;
            case "left":
                break;
            case "right":
                break;
            default:
                Debug.Log(string.Format("error in direction, {0} not identified", First));
                break;
        }
    }

    //public bool UpTouched;
    //public bool DownTouched;
    //public bool LeftTouched;
    //public bool RightTouched;

    //public bool PrevUp;
    //public bool PrevDown;
    //public bool PrevLeft;
    //public bool PrevRight;

    //public bool FirstUp;
    //public bool FirstDown;
    //public bool FirstLeft;
    //public bool FirstRight;

    //public bool LastUp;
    //public bool LastDown;
    //public bool LastLeft;
    //public bool LastRight;

    //public float Timer = 0f;

    Vector2 VectorDeDireccionDeLijado;
    Vector2 VectorDeDireccionDeDesgaste;

    Material EsteMaterial;

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        //if (Timer > 10)
        //{
        //    Timer = 0f;
        //    UpdateMaterial();
        //}
        //Timer += Time.fixedDeltaTime;

        //if (!FirstDown && !FirstLeft && !FirstRight && !FirstUp)
        //{
        //    FirstUp = UpTouched;
        //    FirstDown = DownTouched;
        //    FirstLeft = LeftTouched;
        //    FirstRight = RightTouched;
        //}

        //if (UpTouched) PrevUp = true;
        //if (DownTouched) PrevDown = true;
        //if (LeftTouched) PrevLeft = true;
        //if (RightTouched) PrevRight = true;

        //if (PrevUp && PrevDown && PrevLeft && AlreadyLast()) LastRight = true;
        //if (PrevUp && PrevDown && PrevRight && AlreadyLast()) LastLeft = true;
        //if (PrevUp && PrevLeft && PrevRight && AlreadyLast()) LastDown = true;
        //if (PrevDown && PrevLeft && PrevRight && AlreadyLast()) LastUp = true;

        //if (PrevUp && PrevDown && PrevLeft && PrevRight)
        //{

        //    VectorDeDireccionDeLijado = new Vector2();

        //    UpdateMaterial();

        //    UpTouched = false;
        //    DownTouched = false;
        //    LeftTouched = false;
        //    RightTouched = false;

        //    FirstUp = false;
        //    FirstDown = false;
        //    FirstLeft = false;
        //    FirstRight = false;

        //    PrevUp = false;
        //    PrevDown = false;
        //    PrevLeft = false;
        //    PrevRight = false;
        //}
    }

    void UpdateMaterial()
    {
        //EsteMaterial.SetVector("AAAAAQUIVAUNNOMBREEEEE", VectorDeDireccionDeLijado);
        //EsteMaterial.SetVector("AAAAAQUIVAUNNOMBREEEEE2", VectorDeDireccionDeDesgaste);
    }

    //bool AlreadyLast()
    //{
    //    return !(LastUp || LastDown || LastDown || LastRight);
    //}

    public void SetVar(string name, bool value)
    {
        switch (name)
        {
            case "UpTouched":
                UpTouched = value;
                break;
            case "DownTouched":
                DownTouched = value;
                break;
            case "LeftTouched":
                LeftTouched = value;
                break;
            case "RightTouched":
                RightTouched = value;
                break;
            default:
                Debug.Log(string.Format("error in direction, {0} not identified", First));
                break;
        }
    }

}
