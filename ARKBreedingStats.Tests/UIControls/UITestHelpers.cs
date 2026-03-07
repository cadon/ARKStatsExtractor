using System;
using System.Threading;
using System.Windows.Forms;

namespace ARKBreedingStats.Tests.UIControls
{
    /// <summary>
    /// Helper utilities for UI control testing
    /// </summary>
    public static class UITestHelpers
    {
        /// <summary>
        /// Run an action on the UI thread (STA)
        /// </summary>
        public static void RunOnUIThread(Action action)
        {
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            {
                action();
            }
            else
            {
                var thread = new Thread(() => action())
                {
                    IsBackground = true
                };
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();
            }
        }

        /// <summary>
        /// Run a function on the UI thread and return result
        /// </summary>
        public static T RunOnUIThread<T>(Func<T> func)
        {
            T result = default;
            RunOnUIThread(() => result = func());
            return result;
        }

        /// <summary>
        /// Simulate typing text into a control character by character
        /// </summary>
        public static void SimulateTyping(Control control, string text)
        {
            foreach (char c in text)
            {
                var keyEventArgs = new KeyPressEventArgs(c);
                // This is a simplified simulation - real keyboard events are more complex
                if (control is TextBox textBox)
                {
                    textBox.Text += c;
                }
                Application.DoEvents();
            }
        }

        /// <summary>
        /// Wait for a condition to be true, with timeout
        /// </summary>
        public static bool WaitForCondition(Func<bool> condition, int timeoutMs = 5000, int pollIntervalMs = 100)
        {
            var endTime = DateTime.Now.AddMilliseconds(timeoutMs);
            
            while (DateTime.Now < endTime)
            {
                if (condition())
                {
                    return true;
                }

                Thread.Sleep(pollIntervalMs);
                Application.DoEvents();
            }

            return false;
        }

        /// <summary>
        /// Get all text from a control and its children (useful for debugging)
        /// </summary>
        public static string GetAllText(Control control)
        {
            var text = control.Text;
            foreach (Control child in control.Controls)
            {
                text += Environment.NewLine + GetAllText(child);
            }
            return text;
        }

        /// <summary>
        /// Find a control by name in a control hierarchy
        /// </summary>
        public static T FindControl<T>(Control parent, string name) where T : Control
        {
            if (parent.Name == name && parent is T typedControl)
            {
                return typedControl;
            }

            foreach (Control child in parent.Controls)
            {
                var found = FindControl<T>(child, name);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        /// <summary>
        /// Simulate a mouse click on a control
        /// </summary>
        public static void SimulateClick(Control control, MouseButtons button = MouseButtons.Left)
        {
            var mouseDown = new MouseEventArgs(button, 1, 0, 0, 0);
            var mouseUp = new MouseEventArgs(button, 1, 0, 0, 0);

            // Find the OnMouseDown/OnMouseUp methods and invoke them
            // Note: This is simplified - real mouse events involve more complexity
            var mouseDownMethod = control.GetType().GetMethod("OnMouseDown", 
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var mouseUpMethod = control.GetType().GetMethod("OnMouseUp", 
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            mouseDownMethod?.Invoke(control, new object[] { mouseDown });
            Application.DoEvents();
            mouseUpMethod?.Invoke(control, new object[] { mouseUp });
            Application.DoEvents();

            // For buttons, also trigger Click
            if (control is Button btn)
            {
                btn.PerformClick();
                Application.DoEvents();
            }
        }

        /// <summary>
        /// Get the value of a private field via reflection (useful for testing internal state)
        /// </summary>
        public static T GetPrivateField<T>(object obj, string fieldName)
        {
            var field = obj.GetType().GetField(fieldName, 
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            
            if (field == null)
            {
                throw new ArgumentException($"Field '{fieldName}' not found on type {obj.GetType().Name}");
            }

            return (T)field.GetValue(obj);
        }

        /// <summary>
        /// Set the value of a private field via reflection (use sparingly in tests)
        /// </summary>
        public static void SetPrivateField(object obj, string fieldName, object value)
        {
            var field = obj.GetType().GetField(fieldName, 
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            
            if (field == null)
            {
                throw new ArgumentException($"Field '{fieldName}' not found on type {obj.GetType().Name}");
            }

            field.SetValue(obj, value);
        }

        /// <summary>
        /// Invoke a private method via reflection
        /// </summary>
        public static object InvokePrivateMethod(object obj, string methodName, params object[] parameters)
        {
            var method = obj.GetType().GetMethod(methodName, 
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            
            if (method == null)
            {
                throw new ArgumentException($"Method '{methodName}' not found on type {obj.GetType().Name}");
            }

            return method.Invoke(obj, parameters);
        }
    }
}
