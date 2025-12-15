using UnityEngine;
using UnityEngine.UIElements;


public class GUIManager : MonoBehaviour
{
    // To-Set Properties
    [SerializeField] public string playerTag = "Player";
    [SerializeField] public string hoverLabelIdentifier;
    [SerializeField] public string playerLabelIdentifier;


    // Properties
    private GameObject playerObject;
    private Player player;


    // UI elements
    private Label hoverLabel;
    private Label playerLabel;


    // Utility Methods
    void AssignHoverLabel(Vector2Int OldPoint, Vector2Int NewPoint)  // Updates label whenever hover point is changed
    {
        bool isOnBoard = NewPoint.x != -1 &&  NewPoint.y != -1;
        var rowText = NewPoint.x.ToString();
        var columnText = NewPoint.y.ToString();
        var hoverText = isOnBoard ? $"({rowText}, {columnText})" : "-";
        hoverLabel.text = $"Hover Point: {hoverText}";
    }
    void AssignPlayerLabel(Vector2Int OldPoint, Vector2Int NewPoint)  // Updates label whenever player point is changed
    {
        bool isOnBoard = NewPoint.x != -1 &&  NewPoint.y != -1;
        var rowText = NewPoint.x.ToString();
        var columnText = NewPoint.y.ToString();
        var playerText = isOnBoard ? $"({rowText}, {columnText})" : "-";
        playerLabel.text = $"Player Point: {playerText}";
    }


    // Override Methods
    void Start()
    {
        // Get UI elements
        var document = GetComponent<UIDocument>();
        var root = document.rootVisualElement;
        hoverLabel = root.Q<Label>(hoverLabelIdentifier);
        playerLabel = root.Q<Label>(playerLabelIdentifier);


        // Get player & bind to it's relevant delegates
        playerObject = GameObject.FindWithTag(playerTag);
        if (playerObject.TryGetComponent<Player>(out Player player))
        {
            player.OnHoverPointUpdated += AssignHoverLabel;
            player.OnCurrentPointUpdated += AssignPlayerLabel;
            AssignPlayerLabel(Vector2Int.zero, player.currentPoint);
        }

    }
}
