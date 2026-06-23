using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStompMovement : MonoBehaviour
{
    public Transform PlayerLoc;
    public int PlayerPosValue = 0; //Numero que indica en que paso del camino está el jugador (empieza en 0)
    public Transform CurrentDesignedLoc; //Localizacion designada actual, está para que el jugador no pueda avanzar antes de llegar al punto al que ya se estaba moviendo (va a ser util para cuando el movimiento no sea instantaneo)
    public Transform RailParentOffset; //El transform del objeto que se mueve para que el jugador y los puntos de movimiento cambien de carril
    #region Temporary transform locations 
    public Transform TR1;   //los transform de las ubicaciones del movimiento (no supe como hacerlo con menos fuerza bruta y en el momento que escribo esto no da tiempo para cambiarlo)
    public Transform TR2;
    public Transform TR3;
    public Transform TR4;
    public Transform TR5;
    public Transform TR6;
    public Transform TR7;
    public Transform TR8;
    public Transform TR9;
    public Transform TR10;
    public Transform TR11;
    #endregion
    public Vector3 ObjectiveTransform;
    public Vector3 LastLocation; //está para guardar las coordenadas anteriores del jugador y retrocederlo si es que recibe daño (actualmente funciona devolviendo al jugador a donde estaba antes de tomar daño, lo cual seria en resumen tomar daño y no moverse)
    public Transform TurningPoint1;
    public Transform TurningPoint2;
    public enum LastMove  //determina cual fue el ultimo tipo de movimiento que hizo el jugador esto para devolverlo al tomar daño
    {
        None,
        Walk,
        RailChangeL,
        RailChangeR
    }
    public LastMove LastMovement; //lo mismo que antes, son raros los enums
    public int RailPosition; // representan numericamente de izquierda a derecha en que carril esta el jugador, 0 es el de la izquierda, 1 es el del centro y 2 es el de la derecha

    public enum MovementState
    {
        Static,
        Moving,
        Damage,
        Stumble

    }
    public MovementState MoveState; 

    private void Start()
    {
        LastMovement = LastMove.None; //lastmove esta en ninguno al principio porque todavia no se efectua el primer movimiento
        RailPosition = 1; // el jugador empieza en el riel del centro, van de 0 a 2 (izquierda a derecha)
    }
    private void Update()
    {
        #region MOVEMENT INPUTS (NEW)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            KeyMovementExecINITIAL(1, TR1);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            KeyMovementExec(2, TR2);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            KeyMovementExec(3, TR3);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            KeyMovementExec(4, TR4);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            KeyMovementExec(5, TR5);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            KeyMovementExec(6, TR6);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            KeyMovementExec(7, TR7);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            KeyMovementExec(8, TR8);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            KeyMovementExec(9, TR9);
        }
        if (Input.GetKeyDown(KeyCode.Semicolon))
        {
            KeyMovementExec(10, TR10);
        }
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            KeyMovementExec(11, TR11);
        }
        #endregion
        #region RAIL CHANGE INPUTS
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    RailOffsetterL(1);
        //}
        //if (Input.GetKeyDown(KeyCode.X))
        //{
        //    RailOffsetterR(2);
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha4))
        //{
        //    RailOffsetterL(3);
        //}
        //if (Input.GetKeyDown(KeyCode.V))
        //{
        //    RailOffsetterR(4);
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha6))
        //{
        //    RailOffsetterL(5);
        //}
        //if (Input.GetKeyDown(KeyCode.N))
        //{
        //    RailOffsetterR(6);
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha8))
        //{
        //    RailOffsetterL(7);
        //}
        //if (Input.GetKeyDown(KeyCode.Comma))
        //{
        //    RailOffsetterR(8);
        //    Debug.Log("Comma");
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha0))
        //{
        //    RailOffsetterL(9);
        //}
        //if (Input.GetKeyDown(KeyCode.Slash))
        //{
        //    RailOffsetterR(10);
        //    Debug.Log("underscore/guion");
        //}
        //if (Input.GetKeyDown(KeyCode.Equals))
        //{
        //    RailOffsetterL(11);
        //    Debug.Log("Interrogacion Inicial");
        //}
        #endregion
        if(PlayerPosValue == 11)
        {
            SceneManager.LoadScene("LVL2");
        }
    }

    #region Cosas relacionadas al daño
    public void DamageCondition() // Este script por el momento solo tiene un debug log, pero es el script donde se bajaria la barra de vida al recibir daño, se gatilla siempre 
    {
        Debug.Log("DamageCondition");
        
    }

    private void OnTriggerEnter(Collider other) // detecta cuando el objeto del jugador (la capsula) entra a un trigger que causa que reciba daño
    {
        Debug.Log("Entered Trigger");
        DamageCondition();
        if (LastMovement == LastMove.Walk) //si es que se entra al trigger despues de haber caminado
        {
            PlayerPosValue--;
            PlayerLoc.SetPositionAndRotation(LastLocation, Quaternion.identity); // se devuelve a la posicion en la que estaba antes de chocar con el trigger
            Debug.Log("Walk Damage");
        }
        else if (LastMovement == LastMove.RailChangeR) // si es que se entra al trigger despues de moverse al carril derecho
        {
            RailParentOffset.SetLocalPositionAndRotation(RailParentOffset.localPosition += new Vector3(-1, 0, 0), Quaternion.identity); // se devuelve a la posicion en la que estaba antes de chocar con el trigger
            Debug.Log("Offset R Damage");
        }
        else if (LastMovement == LastMove.RailChangeL) // si es que se entra al trigger despues de moverse al carril izquierdo
        {
            RailParentOffset.SetLocalPositionAndRotation(RailParentOffset.localPosition += new Vector3(1, 0, 0), Quaternion.identity); // se devuelve a la posicion en la que estaba antes de chocar con el trigger
            Debug.Log("Offset L Damage");
        }
    }
    #endregion

    #region cosas relacionadas al cambio de carril
    //public void RailOffsetterR(int KeyValue2)
    //{
    //    if (KeyValue2 == PlayerPosValue && RailPosition != 2)
    //    {
    //        OffsetR();
    //        Debug.Log("Offset R");
    //        LastMovement = LastMove.RailChangeR;
    //        RailPosition++;
    //    }
    //    else
    //    {
    //        DamageCondition();
    //    }
    //}
    //public void RailOffsetterL(int KeyValue)
    //{
    //    if (KeyValue == PlayerPosValue && RailPosition != 0)
    //    {
    //        OffsetL();
    //        Debug.Log("Offset L");
    //        LastMovement = LastMove.RailChangeL;
    //        RailPosition--;
    //    }
    //    else
    //    {
    //        DamageCondition();
    //    }
    //}
    public void OffsetL()
    {
        RailParentOffset.SetLocalPositionAndRotation(RailParentOffset.localPosition += new Vector3(-1, 0, 0), RailParentOffset.localRotation);

    }
    public void OffsetR()
    {
        RailParentOffset.SetLocalPositionAndRotation(RailParentOffset.localPosition += new Vector3(1, 0, 0), RailParentOffset.localRotation);

    }
    #endregion

    #region cosas relacionadas al movimiento 
    public void KeyMovementExec(int KeyValue, Transform TRDesiredPos) // la funcion con parametros que se llama cuando se presiona una tecla de movimiento
    {
        if ((KeyValue -= PlayerPosValue) != 1)
        {
            DamageCondition();
        }
        else if (PlayerLoc.position == CurrentDesignedLoc.position)
        {
            LastLocation = PlayerLoc.position;
            CurrentDesignedLoc.position = TRDesiredPos.position;
            PlayerPosValue++;
            MoveSuccess(TRDesiredPos);
        }
    }
    public void KeyMovementExecINITIAL(int KeyValue, Transform TRDesiredPos) // la funcion con parametros que se llama cuando se presiona la primera tecla de movimiento (es ligeramente distinta a la anterior)
    {
        if ((KeyValue -= PlayerPosValue) != 1)
        {
            DamageCondition();
        }
        else if (PlayerLoc.position != CurrentDesignedLoc.position)
        {
            LastLocation = PlayerLoc.position;
            CurrentDesignedLoc.position = TRDesiredPos.position;
            PlayerPosValue++;
            MoveSuccess(TRDesiredPos);
        }
    }
    public void MoveSuccess(Transform ObjectiveLoc) //funcion que mueve al personaje cuando se cumplen las condiciones (la hice como funcion separada para que sea mas facil editarla al momento de hacer que el movimiento sea mas fluido)
    {
        PlayerLoc.position = ObjectiveLoc.position; 
        //ObjectiveTransform = ObjectiveLoc.position;
        LastMovement = LastMove.Walk;
    }
    #endregion

}
