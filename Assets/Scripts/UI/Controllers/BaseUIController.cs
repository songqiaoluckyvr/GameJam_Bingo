using UnityEngine;
using UnityEngine.UIElements;

namespace BingoGame.UI.Controllers
{
    /// <summary>
    /// Base class for all UI controllers in the game
    /// Provides common functionality for accessing UI elements
    /// </summary>
    public abstract class BaseUIController : MonoBehaviour
    {
        // Reference to the UIDocument component
        protected UIDocument document;
        
        // Reference to the root visual element
        protected VisualElement root;

        /// <summary>
        /// Called when the script instance is being loaded
        /// Sets up the UIDocument and root element references
        /// </summary>
        protected virtual void Awake()
        {
            // Get the UIDocument component
            document = GetComponent<UIDocument>();
            if (document == null)
            {
                Debug.LogError("UIDocument component not found on " + gameObject.name);
                return;
            }

            // Get the root visual element
            root = document.rootVisualElement;
            if (root == null)
            {
                Debug.LogError("Root visual element not found in UIDocument on " + gameObject.name);
                return;
            }
        }

        /// <summary>
        /// Shows an error message in the UI
        /// </summary>
        /// <param name="message">The error message to display</param>
        protected void ShowError(string message)
        {
            Debug.LogError(message);
            // Can be overridden by derived classes to show error in UI
        }

        /// <summary>
        /// Shows a status message in the UI
        /// </summary>
        /// <param name="message">The status message to display</param>
        protected void ShowStatus(string message)
        {
            Debug.Log(message);
            // Can be overridden by derived classes to show status in UI
        }
    }
} 