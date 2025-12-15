using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ObstacleEditor : EditorWindow
{
    // Properties
    private ObstacleData obstacleData;


    // UI Elements
    private VisualElement gridRoot;


    // Editor Methods
    [MenuItem("Tools/Obstacle Editor")]
    public static void ShowEditor()
    {
        ObstacleEditor wnd = GetWindow<ObstacleEditor>();
        wnd.titleContent = new GUIContent("ObstacleEditor");
    }


    // UI Methods
    private void RebuildButtonGrid()
    {

        // Clear existing buttons
        gridRoot.Clear();


        // Return if no obstacle data
        if (obstacleData == null || obstacleData.rows == null)
        {
            return;
        }


        // Iterate through rows
        for (int rowIndex = 0; rowIndex < obstacleData.rows.Length; rowIndex++)
        {

            // Skip if invalid row or column
            var row = obstacleData.rows[rowIndex];
            if (row == null || row.columns == null)
            {
                continue;
            }


            // Row container
            VisualElement rowElement = new VisualElement();
            rowElement.style.flexDirection = FlexDirection.Row;


            // Iterate through columns
            for (int colIndex = 0; colIndex < row.columns.Length; colIndex++)
            {
                // Create button
                Toggle cellToggle = new Toggle();
                cellToggle.value = row.columns[colIndex];
                cellToggle.style.width = 24;
                cellToggle.style.height = 24;
                cellToggle.style.marginRight = 0;


                // Assign method to click
                int fixedColIndex = colIndex;   // Created Fixed indices to avoid mutation in callback
                int fixedRowsIndex = rowIndex;  //
                cellToggle.RegisterValueChangedCallback(evt =>
                {
                    var rowToEffect = obstacleData.rows[fixedRowsIndex];
                    rowToEffect.columns[fixedColIndex] = evt.newValue;
                });


                // Add button to row
                rowElement.Add(cellToggle);
            }


            // Add row to grid
            gridRoot.Add(rowElement);
        }
    }


    // Override Methods
    public void CreateGUI()
    {

        // Create Root
        VisualElement root = rootVisualElement;


        // Add an input for Obstacle Data
        ObjectField obstacleField = new ObjectField("Obstacle Data") { objectType = typeof(ObstacleData), allowSceneObjects = false };
        obstacleField.RegisterValueChangedCallback(evt =>
        {
            obstacleData = evt.newValue as ObstacleData;
            RebuildButtonGrid();
        });
        root.Add(obstacleField);


        // Container for toggleable buttons
        gridRoot = new VisualElement();
        gridRoot.style.marginTop = 10;
        root.Add(gridRoot);
    }
}