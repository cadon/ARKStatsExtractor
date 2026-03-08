# UI Control Tests

This folder contains automated UI tests for ARK Smart Breeding WinForms controls.

## Quick Start

### Run All UI Tests
```powershell
dotnet test --filter "FullyQualifiedName~UIControls"
```

### Run Specific Test Class
```powershell
dotnet test --filter "FullyQualifiedName~StatIOTests"
```

## Writing New Tests

### 1. Create Test Class

```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ARKBreedingStats.Tests.UIControls
{
    [TestClass]
    public class MyControlTests : UIControlTestBase
    {
        private MyControl _control;

        protected override void OnSetup()
        {
            _control = new MyControl();
            AddControlToForm(_control);
        }

        protected override void OnTeardown()
        {
            _control?.Dispose();
        }

        [STATestMethod]  // ⚠️ Must use STATestMethod for WinForms!
        public void MyControl_DoSomething_ExpectedResult()
        {
            // Arrange
            _control.Property = "value";

            // Act
            ClickButton(_control.Button);

            // Assert
            Assert.AreEqual("expected", _control.Result);
        }
    }
}
```

### 2. Important Rules

✅ **DO:**
- Inherit from `UIControlTestBase`
- Use `[STATestMethod]` attribute (NOT `[TestMethod]`)
- Add controls to TestForm with `AddControlToForm()`
- Clean up in `OnTeardown()`
- Use helper methods (ClickButton, SetTextBox, etc.)

❌ **DON'T:**
- Use `[TestMethod]` - WinForms requires STA threading
- Create controls without adding to TestForm
- Forget to dispose controls
- Make tests dependent on each other

## Available Helper Methods

### From UIControlTestBase

```csharp
// Control interaction
ClickButton(button)
SetNumericUpDown(nud, value)
SetTextBox(textBox, "text")
SelectComboBoxItem(comboBox, index)
SetCheckBox(checkBox, true)

// Assertions
AssertVisible(control)
AssertNotVisible(control)
AssertEnabled(control)
AssertDisabled(control)

// Timing
WaitForAsync(milliseconds)
```

### From UITestHelpers

```csharp
// Advanced helpers
UITestHelpers.SimulateClick(control)
UITestHelpers.SimulateTyping(control, "text")
UITestHelpers.WaitForCondition(() => condition, timeoutMs)
UITestHelpers.FindControl<T>(parent, "controlName")

// Reflection (use sparingly)
UITestHelpers.GetPrivateField<T>(obj, "fieldName")
UITestHelpers.InvokePrivateMethod(obj, "methodName", args)
```

## File Structure

```
UIControls/
├── UIControlTestBase.cs          // Base class for all UI tests
├── UITestHelpers.cs              // Static helper utilities
├── STATestMethodAttribute.cs     // Custom test attribute for STA threading
├── StatIOTests.cs                // Tests for StatIO control
├── CreatureBoxTests.cs           // Tests for CreatureBox control
├── TamingControlTests.cs         // Tests for TamingControl
└── [YourNewTests].cs            // Add your tests here
```

## Troubleshooting

### "UI control tests must run on STA thread"
**Solution**: Use `[STATestMethod]` instead of `[TestMethod]`

### "Control handle not created"
**Solution**: Make sure you called `AddControlToForm(_control)` in OnSetup

### "NullReferenceException in control"
**Solution**: Control may need more complete initialization data (Species, CreatureCollection, etc.). Check what the control expects.

### Tests are flaky/timing issues
**Solution**: Add `WaitForAsync()` after actions that trigger async operations or debouncers

### Can't find control in test
**Solution**: Verify control is added to TestForm and handle is created. Try `_control.CreateControl()` manually if needed.

## Test Organization

- **Unit Tests**: Test individual methods/properties in isolation
- **Integration Tests**: Test control with real dependencies
- **UI Tests**: Test user interactions end-to-end

Most tests here are **integration tests** - they test controls with some real dependencies. For pure logic, consider extracting to services and writing unit tests.

## Coverage

Current test coverage:
- ✅ StatIO: 20 tests, ~90% coverage
- ✅ CreatureBox: 11 tests, ~70% coverage
- ✅ TamingControl: 13 tests, ~50% coverage
- ⏳ CreatureInfoInput: Needs tests
- ⏳ SpeciesSelector: Needs tests
- ⏳ MultiSetter: Needs tests

## Contributing

When adding UI controls or modifying existing ones:
1. Add/update tests for the control
2. Run all UI tests to ensure no regressions
3. Document any domain logic that should be extracted
4. Consider if logic should be in a service instead of the control

## Further Reading

- [UI Control Specifications](../../docs/UI_CONTROL_SPECIFICATIONS.md)
- [UI Testing Summary](../../docs/UI_TESTING_SUMMARY.md)
