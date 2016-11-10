using UnityEngine;

public class Healthbar : MonoBehaviour
{
    private GameObject bar;

    public void Initialize(Vector3 position)
    {
        GameObject barObject = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Healthbar_bar"));
        barObject.transform.parent = transform;
        barObject.transform.position = position;
        this.bar = barObject.transform.FindChild("Healthbar Anchor").gameObject;
    }

    public void SetHealthBar(float ratio)
    {
        var tempScale = bar.transform.localScale;
        tempScale.x = ratio;

        bar.transform.localScale = tempScale;
    }
}
