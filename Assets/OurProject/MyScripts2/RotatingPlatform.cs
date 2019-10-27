using UnityEngine;
using System.Collections;

public class RotatingPlatform : MonoBehaviour {

	/// <summary>
	/// Enumerado que representa el tipo de plataforma giratoria
	/// </summary>
	public enum RotateType
	{
		ENDLESS = 0,
		PERIODIC = 1,	
	}

	/// <summary>
	/// Tipo de rotación del GameObject
	/// </summary>
	public RotateType m_RotateType = RotateType.ENDLESS;
	
	/// <summary>
	/// Booleano que activa/desactiva la rotación en el eje X
	/// </summary>
	public bool m_RotateX;
	
	/// <summary>
	/// Booleano que activa/desactiva la rotación en el eje Y
	/// </summary>
	public bool m_RotateY;
	
	/// <summary>
	/// Booleano que activa/desactiva la rotación en el eje Z
	/// </summary>
	public bool m_RotateZ;
	
	/// <summary>
	/// Vector que indica la velocidad de rotación en las tres componentes
	/// </summary>
	public Vector3 m_RotationSpeed;
	
	/// <summary>
	/// Tiempo que la plataforma estará parada en el tipo de movimiento PERIODIC
	/// </summary>
	public float m_TimeStopped = 3.0f;

	/// <summary>
	/// Cantidad de grados que la plataforma girara antes de pararse cuando es PERIODIC
	/// </summary>
	private float m_LoopLimit = 180.0f;
	
	/// <summary>
	/// Variable interna para controlar la espera "parado"
	/// </summary>
	private float m_TimeToStop = 0.0f;
	
	/// <summary>
	/// Variable interna para comprobar cuánto se ha rotado la entidad en el eje X
	/// </summary>
	private float m_CurrentXRotation = 0.0f;
	
	/// <summary>
	/// Variable interna para comprobar cuánto se ha rotado la entidad en el eje X
	/// </summary>
	private float m_CurrentYRotation = 0.0f;
	
	/// <summary>
	/// Variable interna para comprobar cuánto se ha rotado la entidad en el eje X
	/// </summary>
	private float m_CurrentZRotation = 0.0f;
	
	/// <summary>
	/// Variable interna que controla el estado parado/en movimiento de la entidad PERIODIC
	/// </summary>
	private bool m_Stop = false;

    public Transform m_globalParent = null; //Por defecto el padre global será la raiz de la escena pero podría ser que no fuera así.
    public Transform m_transformToAttach;
    private Vector3 m_EnterScale = Vector3.one;


    void Start()
    {
        if (m_transformToAttach == null)
            m_transformToAttach = transform;
    }

    /// <summary>
    /// En el update tenemos que girar el GameObject. 
    /// Será necesario hacer distinción de tipos
    /// </summary> Update is called once per frame
    void Update ()
	{
        // TODO 1 - En función del tipo de rotación, llamar a JustRotate() o a RotateAndStop()
        // Pista: switch (m_RotateType) {...}
        if (m_RotateType == RotateType.PERIODIC)
        {
            RotateAndStop();
        }
        else
        {
            JustRotate();
        }

    }
	
	/// <summary>
	/// En caso de tener que llamar a la función que para el gameObject
	/// tenemos que gestionar la espera durante la cual el objeto está "quieto"
	/// </summary>
	void RotateAndStop()
	{
		// Con un booleano controlamos el estado "parado" o "rotando"
		if (m_Stop)
		{
			m_TimeToStop += Time.deltaTime;
			if (m_TimeToStop >= m_TimeStopped)
			{
				m_Stop = false;
				m_TimeToStop = 0.0f;
				m_CurrentXRotation = 0.0f;
				m_CurrentYRotation = 0.0f;
				m_CurrentZRotation = 0.0f;
			}
		}
		else
		{
			if (CheckIfHasRotated())
				m_Stop = true;
			JustRotate();
		}
	}
	
	/// <summary>
	/// Función que comprueba si se ha llegado el limite de rotación establecido
	/// </summary>
	/// <returns>
	/// True si en algún eje se ha rotado más de lo permitido, false en caso contrario <see cref="System.Boolean"/>
	/// </returns>
	bool CheckIfHasRotated()
	{
        // TODO 2 - Retornar si alguna de las rotaciones de los ejes ha sobrepasado o es igual al LoopLimit
        return (m_CurrentXRotation >= m_LoopLimit) ||
                (m_CurrentYRotation >= m_LoopLimit) ||
                (m_CurrentZRotation >= m_LoopLimit);

    }
	
	/// <summary>
	/// Esa función se limita a rotar el GameObject en función de los ejes que haya 
	/// habilitado el usuario.
	/// </summary>
	void JustRotate()
	{
		if (m_RotateX)
		{
			float rotX = _GetRotationAmount(m_RotationSpeed.x, m_CurrentXRotation);
			gameObject.transform.Rotate(rotX, 0, 0);
			m_CurrentXRotation +=rotX;
		}
			
		if (m_RotateY)
		{
			float rotY = _GetRotationAmount(m_RotationSpeed.y, m_CurrentYRotation);
			gameObject.transform.Rotate(0, rotY, 0);
			m_CurrentYRotation +=rotY;
		}
				
		if (m_RotateZ)
		{
			float rotZ = _GetRotationAmount(m_RotationSpeed.z, m_CurrentZRotation);
			gameObject.transform.Rotate(0, 0, rotZ);
			m_CurrentZRotation +=rotZ;
		}
	}
	
	/// <summary>
	/// Función helper para obtener qué rotación hay que aplicar a un objeto
	/// </summary>
	/// <param name="rotationSpeed">
	/// Velocidad de rotación en un eje determinado <see cref="System.Single"/>
	/// </param>
	/// <param name="currentRotation">
	/// Rotación actual en el eje -para los movimientos periódicos- <see cref="System.Single"/>
	/// </param>
	/// <returns>
	/// Rotación que hay que aplicar al objeto <see cref="System.Single"/>
	/// </returns>
	float _GetRotationAmount(float rotationSpeed, float currentRotation)
	{
		float frameRotation = rotationSpeed * Time.deltaTime;
		currentRotation += frameRotation;
		// En caso de que la rotación sea de tipo periódico, queremos que rote exactamente
		// la cantidad de grados indicada en el límite del Loop 
		// Por ejemplo, si es 180 el límite, queremos que rote 180, y no 181 (porque "descuadraría" la vuelta)
		if (m_RotateType == RotateType.PERIODIC)
		{
				if (currentRotation	> m_LoopLimit)
				{
					frameRotation -= currentRotation - m_LoopLimit;
				}
		}
		
		return frameRotation;
	}

    void OnTriggerStay(Collider other)
    {
        Attachable atachable = other.GetComponent<Attachable>();
        if (atachable && atachable.IsAttachable)
        {
            float gradosRotacion = Mathf.Abs(transform.rotation.z);
            Debug.LogError(gradosRotacion);

            if (gradosRotacion > 0.25 || (gradosRotacion > 0.75 ))
            {
                OnTriggerExit(other);
            }
        }
    }


    void OnTriggerEnter(Collider other)
    {
        Debug.LogError("transform.rotation.eulerAngles.z: " + transform.rotation.eulerAngles.z);
        //TODO 1: Cuando el objeto que caiga sea attachable, atachamos el objeto. Ojo, la scala puede cambiar!!!
        //Hacer hijo el personaje de la plataforma
        Attachable atachable = other.GetComponent<Attachable>();
        if (atachable && atachable.IsAttachable)
        {
            m_EnterScale = other.transform.localScale;
            //hay que cambiar el Transform del padre que queremos cambiar
            other.transform.parent = m_transformToAttach;
            atachable.IsAttached = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        //TODO 2: Cuando el objeto que caiga sea attachable, como estamos saliendo, desatachamos el objeto. Ojo, la scala puede cambiar!!!
        //Quitar hijo de la plataforma
        Attachable atachable = other.GetComponent<Attachable>();
        if (atachable && atachable.IsAttached)
        {
            other.transform.parent = m_globalParent;
            other.transform.localScale = m_EnterScale;
            atachable.IsAttached = false;
        }
    }
}
