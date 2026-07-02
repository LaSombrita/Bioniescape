using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovementManager : MonoBehaviour
{
    #region Locations and transforms
    [Header("Locations and transforms")]

    public Transform[] TransformArray;
    public Transform DesiredTargetTransform;
    public Transform RailOriginTransform;
    public Transform PlayerSpriteTransform;
    #endregion

    #region KeyData
    [Header("KeyData")]                                             // KEYS             DATA
    public string CurrentFrameKey;
    public string CurrentFrameKeyLower;
    public int CurrentPosValue;
    public KeyValuesScriptable[] KeyValues;
    #endregion

    #region State machines data
    [Header("State Machines Data")]                                 // STATE            MACHINE
    public bool IsInTrigger;
    public int CurrentRail;
    public float RailOffsetValue; //determina la distancia en metros entre rieles
    public float ElapsedTime;
    public float DamageAnimationTime;
    public enum MovementStates
    {
        Idle,               // estado disponible para cualquier input
        SelectedWaiting,           // estado previo a determinar el movimiento 
        SelectedChoosing,       //Estado despues de waiting en el que se esta tomando la decision con respecto al daño o movimiento
        DamageReturn,       //estado de daño por chocar con un obstaculo
        DamageMisinput,         //estado de daño por presionar la tecla incorrecta
        WalkingToTarget,         //estado en el que el jugador esta caminando a la ubicacion designada
        MovementCooldown         //tiempo de cooldown, no se puede hacer nada
    }
    public enum LastMovement
    {
        MoveL,
        MoveR,
        Advance
    }
    public LastMovement LastMove;
    public MovementStates MoveStates;
    public int HallwayIndex; // representa el numero de pasillo en el que se encuentra, se usa para entrar al minijuego despues de recorrer cierta cantidad de pasillos
    public float SpriteMovementTime; // el tiempo que se demora el sprite en llegar a la siguiente ubicacion al dar un paso exitosamente
    public float SpriteMaxSpeed;
    public Vector3 CurrentVel = new Vector3(0, 0, 0); //usado para la variable de velocidad inicial del movimiento del sprite, cambiar en inspector si es necesario

    #endregion



                                  //            A           W           A           K           E                and more






    private void Awake()
    {
        
    }
    void Update()
    {
        #region Input Reciever and reactors
        MovementReactor();
        
        
        if (Input.anyKeyDown)
        {
            CurrentFrameKey = Input.inputString;
            CurrentFrameKeyLower = CurrentFrameKey.ToLower(); //Asignamos la letra presionada y la convertimos a minuscula

            if(MoveStates == MovementStates.Idle)
            {
                for (int i = 0; i < 40; i++) //Loop que determina el efecto de la tecla presionada
                {
                    if (KeyValues[i].KeyStringRaw == CurrentFrameKeyLower)
                    {
                        
                        if ((KeyValues[i].KeyCoordinateValue == CurrentPosValue && KeyValues[i].RailChangeL == false && KeyValues[i].RailChangeR == false))   //MOVEMENT SUCCESS
                        {
                            
                            MovementCondition(i);


                            break;
                        }
                        else if (KeyValues[i].KeyCoordinateValue > CurrentPosValue) 
                        {
                            MoveStates = MovementStates.DamageReturn;
                            Debug.Log("Damage Condition");
                            break;
                        }
                        else if (KeyValues[i].KeyCoordinateValue == CurrentPosValue && (KeyValues[i].RailChangeR == true)) //RAIL CHANGE CONDITION        R
                        {
                            Debug.Log("Rail R");
                            RailChangeR();
                        }
                        else if (KeyValues[i].KeyCoordinateValue == CurrentPosValue && (KeyValues[i].RailChangeL == true))  //RAIL CHANGE CONDITION        L
                        {
                            Debug.Log("Rail L");
                            RailChangeL();
                        }
                        else if (KeyValues[i].KeyCoordinateValue < CurrentPosValue)
                        {
                            Debug.Log("Damage Condition P");
                            MoveStates = MovementStates.DamageReturn;
                        }
                        else
                        {
                            Debug.Log("No other condition triggered");
                        }
                    }


                }
                if (Input.GetKeyDown(KeyCode.LeftBracket))
                {
                    if (CurrentPosValue == 10)
                    {
                        MovementCondition(10);
                        //Minigame or Hall change Condition
                        //Timer for interaction
                        //adjust pos value to 0
                    }
                    else
                    {
                        Debug.Log("Damage Conition Last");
                    }
                }
            }
            
        }
        #endregion
    }

    public void MovementCondition(int TransformIndex)
    {
        DesiredTargetTransform.SetPositionAndRotation(TransformArray[TransformIndex].position, Quaternion.identity);
        Debug.Log("Movement Condition Executed");
        LastMove = LastMovement.Advance;
        MoveStates = MovementStates.SelectedWaiting;

        
    }
    public void RailChangeL()
    {
        if(CurrentRail > 0 )
        {
            CurrentRail--;
            RailOriginTransform.SetLocalPositionAndRotation(new Vector3(RailOriginTransform.position.x + (-1f * RailOffsetValue), RailOriginTransform.position.y, RailOriginTransform.position.z), Quaternion.identity);
            MoveStates = MovementStates.SelectedWaiting;
            LastMove = LastMovement.MoveL;
        }
        else
        {
            MoveStates = MovementStates.DamageReturn;
        }
    }
    public void RailChangeR()
    {
        if (CurrentRail < 2)
        {
            CurrentRail++;
            RailOriginTransform.SetLocalPositionAndRotation(new Vector3(RailOriginTransform.position.x + RailOffsetValue, RailOriginTransform.position.y, RailOriginTransform.position.z), Quaternion.identity);
            MoveStates = MovementStates.SelectedWaiting;
            LastMove = LastMovement.MoveR;
        }
        else
        {
            MoveStates = MovementStates.DamageReturn;
        }
    }
    public void MovementReactor()
    {
        if (MoveStates == MovementStates.SelectedWaiting)
        {
            ElapsedTime += Time.deltaTime;
            if(ElapsedTime >= 0.1f)
            {
                MoveStates = MovementStates.SelectedChoosing;
                ElapsedTime = 0;
            }
        }
        else if(MoveStates == MovementStates.SelectedChoosing)
        {
            if(IsInTrigger == false && LastMove == LastMovement.Advance)
            {
                MoveStates = MovementStates.WalkingToTarget;
                CurrentPosValue++;
            }
            else if(IsInTrigger == false && LastMove != LastMovement.Advance)
            {
                MoveStates = MovementStates.WalkingToTarget;
            }
            else
            {
                if (LastMove == LastMovement.Advance)
                {
                    DesiredTargetTransform.SetPositionAndRotation(PlayerSpriteTransform.position, Quaternion.identity);
                    MoveStates = MovementStates.DamageReturn;
                }
                else if (LastMove == LastMovement.MoveL)
                {
                    CurrentRail++;
                    RailOriginTransform.SetLocalPositionAndRotation(new Vector3(RailOriginTransform.position.x + RailOffsetValue, RailOriginTransform.position.y, RailOriginTransform.position.z), Quaternion.identity);
                    MoveStates = MovementStates.DamageReturn;
                }
                else if (LastMove == LastMovement.MoveR)
                {
                    CurrentRail--;
                    RailOriginTransform.SetLocalPositionAndRotation(new Vector3(RailOriginTransform.position.x + (-1f * RailOffsetValue), RailOriginTransform.position.y, RailOriginTransform.position.z), Quaternion.identity);
                    MoveStates = MovementStates.DamageReturn;
                }
            }
        }
        else if(MoveStates == MovementStates.DamageReturn) 
        {
            ElapsedTime += Time.deltaTime;
            if(ElapsedTime >= DamageAnimationTime)
            {
                //se llama a la funcion de otro script para registrar el daño
                ElapsedTime = 0;
                Debug.Log("Damage Return");
                MoveStates = MovementStates.Idle;
            }
        }
        else if(MoveStates == MovementStates.WalkingToTarget)
        {
            PlayerSpriteTransform.position = Vector3.SmoothDamp(PlayerSpriteTransform.position, DesiredTargetTransform.position, ref CurrentVel, SpriteMovementTime * Time.deltaTime, SpriteMaxSpeed);
            ElapsedTime += Time.deltaTime;
            if(ElapsedTime > SpriteMovementTime)
            {
                MoveStates = MovementStates.Idle;
                ElapsedTime = 0;
            }
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        IsInTrigger = true;
    }
    private void OnTriggerExit(Collider other)
    {
        IsInTrigger = false;
    }
}



//for (int i = 0; i < 41; i++)
//{
//    if (KeyValues[i].KeyStringRaw == CurrentFrameKeyLower)
//    {
//        Debug.Log("Key Pressed");
//        if ((KeyValues[i].KeyCoordinateValue -= CurrentPosValue) > 1)
//        {
//            //Damage Condition
//            Debug.Log("Damage Condition");
//            break;
//        }
//        else if ((KeyValues[i].KeyCoordinateValue -= CurrentPosValue) == 1)
//        {

//            CurrentPosValue++;
//            MovementCondition(i);
//            Debug.Log("Movement Condition");
//            break;
//        }
//        else if ((KeyValues[i].KeyCoordinateValue -= CurrentPosValue) == 1 && (KeyValues[i].RailChanger == true))
//        {
//            if (KeyValues[i].KeyCoordinateValue == 1 || KeyValues[i].KeyCoordinateValue == 3 || KeyValues[i].KeyCoordinateValue == 5 || KeyValues[i].KeyCoordinateValue == 7 || KeyValues[i].KeyCoordinateValue == 9)
//            {
//                //Rail change L
//                Debug.Log("Rail Change L");
//                break;
//            }
//            else if (KeyValues[i].KeyCoordinateValue == 2 || KeyValues[i].KeyCoordinateValue == 4 || KeyValues[i].KeyCoordinateValue == 6 || KeyValues[i].KeyCoordinateValue == 8 || KeyValues[i].KeyCoordinateValue == 0)
//            {
//                //Rail change R
//                Debug.Log("Rail Change R");
//                break;
//            }
//            else
//            {
//                //Damage Condition
//                Debug.Log("Damage Condition");
//                break;
//            }
//        }
//        else
//        {
//            Debug.Log("Damage condition agaaaaain");
//        }
//    }


//}