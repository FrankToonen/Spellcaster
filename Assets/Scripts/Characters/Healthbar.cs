using System.Collections;
using UnityEngine;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private GameObject anchor;
    [SerializeField] private SpriteRenderer barRenderer;
    [SerializeField] private TextMesh damageText;
    [SerializeField] private TextMesh healthText;

    /// <summary>
    /// Sets the position and color of the healthbar.
    /// Also sets the damage text event.
    /// </summary>
    /// <param name="position">The position for the healthbar.</param>
    /// <param name="color">The color of the bar.</param>
    public void Initialize(Vector3 position, Color color)
    {
        transform.position = position;

        barRenderer.color = color;

        var t = damageText.GetComponent<AnimationExtension>();
        t.OnAnimationFinished += () => { damageText.gameObject.SetActive(false); };
    }
    
    /// <summary>
    /// Sets the size of the healthbar based on the ratio of the given numbers.
    /// </summary>
    /// <param name="currentHealth">The health to display.</param>
    /// <param name="maxHealth">The maximum health to determine the size of the bar from.</param>
    public void SetHealthBar(int currentHealth, int maxHealth)
    {
        healthText.text = currentHealth + " / " + maxHealth;

        var ratio = (float)currentHealth / maxHealth;
        StartCoroutine(LerpScale(ratio));
    }

    /// <summary>
    /// Shows an animation of the amount of damage dealt.
    /// </summary>
    /// <param name="damage"></param>
    public void ShowDamage(int damage)
    {
        var text = damage >= 0 ? "-" + damage : "+" + (damage*-1);

        damageText.text = text;
        damageText.gameObject.SetActive(true);
        damageText.GetComponent<Animation>().Play();
    }

    /// <summary>
    /// Lerp the scale of the healthbar to visually display the amount of health left.
    /// </summary>
    /// <param name="xTargetScale">The scale on the X-axis.</param>
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
