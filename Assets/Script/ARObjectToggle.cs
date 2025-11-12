using UnityEngine;

public class ARObjectToggle : MonoBehaviour
{

    [SerializeField]
    private MeshRenderer MeshRendererToToggle;
    
    public void ToggleObject()
    {
        MeshRendererToToggle.enabled = !MeshRendererToToggle.enabled;

        // if(MeshRendererToToggle.enabled)
        // {
        //     MeshRendererToToggle.enabled = false;
        // }
        // else
        // {
        //     MeshRendererToToggle.enabled = true;
        // }
    }
}
