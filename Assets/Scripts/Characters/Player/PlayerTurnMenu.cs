using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTurnMenu : MonoBehaviour
{
    [SerializeField] private GameObject optionSelect;
    [SerializeField] private GameObject attackSelect;
    [SerializeField] private GameObject targetSelect;

    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform attackLayoutObject;
    [SerializeField] private Transform itemLayoutObject;
    [SerializeField] private Button confirmButton;

    private readonly List<Button> attackButtons = new List<Button>();
    private readonly List<Button> itemButtons = new List<Button>();

    private GameObject openMenu;
    private Character attackingCharacter;
    private BaseAttack selectedAttack;
    private List<Character> selectedTargets;
    
    /// <summary>
    /// Opens on the correct menu.
    /// </summary>
    private void OnEnable()
    {
        targetSelect.SetActive(false);
        attackSelect.SetActive(false);
        OpenMenu(optionSelect);
    }

    /// <summary>
    /// Creates the clickable buttons for selecting an attack.
    /// </summary>
    /// <param name="attacks">The attacks to create button for.</param>
    /// <param name="user">The character that will perform the attacks.</param>
    public void CreateAttackButtons(BaseAttack[] attacks, Character user)
    {
        attackingCharacter = user;

        // Remove existing buttons to prevent duplicates.
        foreach (var button in attackButtons)
        {
            Destroy(button.gameObject);
        }
        attackButtons.Clear();

        // Add a button to the object with the vertical layout group for each attack the player can perform.
        foreach (var attack in attacks)
        {
            var button = CreateButton(attackLayoutObject, attack.info.name + "\n" + attack.info.AttackText());

            attackButtons.Add(button);
            button.interactable = attack.CanBeUsed(attackingCharacter);

            // Add attack select to the OnClick event.
            var attackRef = attack;
            button.onClick.AddListener(delegate
            {
                SelectAttack(attackRef);

                // Open the target menu to select a target and/or confirm the attack.
                OpenMenu(targetSelect);
            });
        }
    }

    /// <summary>
    /// Creates the clickable buttons for selecting an item to use.
    /// TODO: This method is pretty much copy+paste of CreateAttackButtons. Maybe find a way to merge them.
    /// </summary>
    /// <param name="items">The list of Items to make buttons for.</param>
    /// <param name="user">The character that will perform the attacks.</param>
    public void CreateItemButtons(List<Item> items, Character user)
    {
        // Remove existing buttons to prevent duplicates.
        foreach (var button in itemButtons)
        {
            Destroy(button.gameObject);
        }
        itemButtons.Clear();

        // Add a button to the object with the vertical layout group for each attack the player can perform.
        foreach (var item in items)
        {
            var button = CreateButton(itemLayoutObject, item.ToString());
            itemButtons.Add(button);
            button.interactable = item.amount > 0;

            var itemRef = item;
            button.onClick.AddListener(delegate
            {
                itemRef.UseItem(user);
                BattleManager.instance.EndTurn();
                gameObject.SetActive(false);
            });
        }
    }

    /// <summary>
    /// Creates a button to select an attack.
    /// </summary>
    private static Button CreateButton(Transform parent, string buttonText)
    {
        var buttonObject = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Button"), parent);
        buttonObject.transform.SetAsFirstSibling();
        buttonObject.transform.localScale = Vector3.one;
        buttonObject.GetComponentInChildren<Text>().text = buttonText;

        return buttonObject.GetComponent<Button>();  
    }

    /// <summary>
    /// Sets the confirm button's state and text.
    /// </summary>
    /// <param name="interactable">Whether the button is interactable or not.</param>
    private void SetConfirmButton(bool interactable)
    {
        confirmButton.interactable = interactable;
        var text =  confirmButton.interactable ? "Confirm" : "Select target";
        confirmButton.GetComponentInChildren<Text>().text = text;
    }

    /// <summary>
    /// Opens the target select menu in which the target for the attack can be selected.
    /// </summary>
    /// <returns>How many targets were highlighted.</returns>
    private void HighlightTargets(IEnumerable<Character> availableTargets, Character.HighlightType type)
    {
        foreach (var target in availableTargets)
        {
            target.SetHighlight(type);
            
            // Add a listener to the OnHighlightClicked event of the target options.
            if (type == Character.HighlightType.Option)
            {
                target.OnHighlightClicked += SelectTarget;
            }
            else
            {
                target.OnHighlightClicked -= SelectTarget;
            }
        }
    }

    /// <summary>
    /// Sets the selected attack to the one provided.
    /// This will use that attack's info to highlight targets.
    /// It will also open the confirmation menu.
    /// </summary>
    /// <param name="attack">The selected attack.</param>
    private void SelectAttack(BaseAttack attack)
    {
        selectedAttack = attack;

        // Find the targets and highlight them correctly.
        var availableTargets = BattleManager.instance.GetCharactersOfType(Character.TargetToCharacterType(attackingCharacter.characterType, selectedAttack.info.targetType));
        var selected = availableTargets.Count == 1 || selectedAttack.info.targetType == AttackInfo.TargetType.AllEnemy 
            || selectedAttack.info.targetType == AttackInfo.TargetType.AllFriendly 
            || selectedAttack.info.targetType == AttackInfo.TargetType.Everyone 
            || selectedAttack.info.targetType == AttackInfo.TargetType.None;

        HighlightTargets(availableTargets, selected ? Character.HighlightType.Selected : Character.HighlightType.Option);

        // If the targets are deemed selected, set the attack's targets.
        if (selected)
        {
            selectedTargets = availableTargets.Count == 0 ? new List<Character> { attackingCharacter } : availableTargets;
        }

        // Set the state of the confirm button based on the highlight status.
        SetConfirmButton(selected);
    }

    /// <summary>
    /// Sets the target for the attack.
    /// </summary>
    /// <param name="target">The target.</param>
    public void SelectTarget(Character target)
    {
        // Reverse the previous selection.
        if (selectedTargets != null)
        {
            // This will reset the highlights for the available targets.
            SelectAttack(selectedAttack);
        }

        // Make new selection.
        selectedTargets = new List<Character> { target };
        HighlightTargets(selectedTargets, Character.HighlightType.Selected);

        SetConfirmButton(true);
    }

    /// <summary>
    /// Removes highlights and the selected target.
    /// </summary>
    public void CancelAttack()
    {
        HighlightTargets(BattleManager.instance.GetCharactersOfType(Character.CharacterType.All), Character.HighlightType.None);
        selectedAttack = null;
        selectedTargets = null;
    }

    /// <summary>
    /// Performs the selected attack on the specified target.
    /// </summary>
    public void PerformAttack()
    {
        attackingCharacter.PerformAttack(selectedAttack, selectedTargets);

        CancelAttack();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Switches the opened menu to the specified menu.
    /// </summary>
    /// <param name="menu">The menu to open.</param>
    public void OpenMenu(GameObject menu)
    {
        // Close the previous menu.
        if (openMenu != null)
        {
            openMenu.SetActive(false);
        }

        // Open the new menu.
        menu.SetActive(true);
        openMenu = menu;
    }
}
