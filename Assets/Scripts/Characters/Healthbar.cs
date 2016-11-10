using System.Collections;
using UnityEngine;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private GameObject anchor;
    [SerializeField] private SpriteRenderer barRenderer;
    [SerializeField] private TextMesh damageText;
    [SerializeField] private TextMesh healthText;

    public void Initialize(Vector3 position, Color color)
    {
        transform.position = position;

        barRenderer.color = color;

        var t = damageText.GetComponent<AnimationExtension>();
        t.OnAnimationFinished += () => { damageText.gameObject.SetActive(false); };
    }

    public void SetHealthBar(int currentHealth, int maxHealth)
    {
        healthText.text = currentHealth + " / " + maxHealth;

        var ratio = (float)currentHealth / maxHealth;
        StartCoroutine(LerpScale(ratio));
    }

    public void ShowDamage(int damage)
    {
        var text = damage >= 0 ? "-" + damage : "+" + (damage*-1);

        damageText.text = text;
        damageText.gameObject.SetActive(true);
        damageText.GetComponent<Animation>().Play();
    }

    private IEnumerator LerpScale(float xTargetScale)
    {
        var tempScale = anchor.transform.localScale;
        tempScale.x = xTargetScale;

        var endTime = Time.time + 0.5f;

        while (Time.time < endTime)
        {
            anchor.transform.localScale = Vector3.Lerp(anchor.transform.localScale, tempScale, 5*Time.deltaTime);
            
            yield return null;
        }

        tempScale.x = xTargetScale;
        anchor.transform.localScale = tempScale;
    }
}
