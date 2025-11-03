using UnityEngine;

public class PistolAnim : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject oldParent;
    
    public void ParentNull()
    {
        transform.parent = null;
    }
   public  void Parent()
    {
        transform.parent = oldParent.transform;
    }
}
