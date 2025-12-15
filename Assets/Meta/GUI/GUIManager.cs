using UnityEngine;
using UnityEngine.UIElements;

public class GUIManager : MonoBehaviour
{
    // UI elements
    private Label hoverLabel;


    // Properties
    [SerializeField] public string playerTag;
    [SerializeField] public string hoverLabelIdentifier;
    private GameObject playerObject;
    private Player player;


    // Utility Methods
    void AssignHoverLabel(Vector2Int OldLocation, Vector2Int NewLocation)  // Updates label whenever hover location is changed
    {
        bool isOnBoard = NewLocation.x != -1 &&  NewLocation.y != -1;
        var rowText = NewLocation.x.ToString();
        var columnText = NewLocation.y.ToString();
        var HoverText = isOnBoard ? $"({rowText}, {columnText})" : "-";
        hoverLabel.text = $"Hover Location: {HoverText}";
    }


    // Override Methods
    void Start()
    {
        // Get player & bind to it's relevant delegates
        playerObject = GameObject.FindWithTag(playerTag);
        if (playerObject.TryGetComponent<Player>(out Player player))
        {
            player.OnHoverLocationUpdated += AssignHoverLabel;
        }


        // Get UI elements
        var document = GetComponent<UIDocument>();
        var root = document.rootVisualElement;
        hoverLabel = root.Q<Label>(hoverLabelIdentifier);
    }

}
