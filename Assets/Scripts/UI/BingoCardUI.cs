using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace BrilliantBingo.Code.Infrastructure.UI
{
    public class BingoCardUI : MonoBehaviour
    {
        private VisualElement m_Root;
        private Label[,] m_NumberLabels;
        private VisualElement[,] m_Cells;
        private HashSet<int> m_MarkedNumbers = new HashSet<int>();

        private void OnEnable()
        {
            m_Root = GetComponent<UIDocument>().rootVisualElement;
            InitializeGridReferences();
        }

        private void InitializeGridReferences()
        {
            m_NumberLabels = new Label[5, 5];
            m_Cells = new VisualElement[5, 5];

            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    string cellName = $"cell-{row}-{col}";
                    string numberName = $"number-{row}-{col}";
                    
                    m_Cells[row, col] = m_Root.Q<VisualElement>(cellName);
                    m_NumberLabels[row, col] = m_Root.Q<Label>(numberName);

                    // Add click event handler
                    int capturedRow = row;
                    int capturedCol = col;
                    m_Cells[row, col].RegisterCallback<ClickEvent>(evt => OnCellClicked(capturedRow, capturedCol));
                }
            }
        }

        public void SetNumber(int row, int col, int number)
        {
            if (row < 0 || row >= 5 || col < 0 || col >= 5) return;
            if (row == 2 && col == 2) return; // Skip FREE space

            m_NumberLabels[row, col].text = number.ToString();
        }

        private void OnCellClicked(int row, int col)
        {
            if (row == 2 && col == 2) return; // Skip FREE space

            var cell = m_Cells[row, col];
            var number = int.Parse(m_NumberLabels[row, col].text);

            if (m_MarkedNumbers.Contains(number))
            {
                m_MarkedNumbers.Remove(number);
                cell.RemoveFromClassList("marked");
            }
            else
            {
                m_MarkedNumbers.Add(number);
                cell.AddToClassList("marked");
            }
        }

        public void ClearBoard()
        {
            m_MarkedNumbers.Clear();
            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    if (row == 2 && col == 2) continue; // Skip FREE space
                    m_NumberLabels[row, col].text = "";
                    m_Cells[row, col].RemoveFromClassList("marked");
                }
            }
        }

        public bool IsNumberMarked(int number)
        {
            return m_MarkedNumbers.Contains(number);
        }
    }
} 