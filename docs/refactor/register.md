
# What is being changed?
> **Date:** 10/02/24
>
> **Author** Andreas Tietgen

The task is to convert the code from python to C# as best as possible without refactoring the code too much. That is, the code should look like a 1:1 conversion as much as possible.

## What did we do?
1. Created the `Register.cshtml` and added the same UI logic that is present in the corresponding python template `register.html`

2. Then the `Register.cshtml.cs` page model that handles the request to the site was created, with the model that is necessary in order to register

3. We tested the site if it worked as it should but figured that the page model did not capture what was typed because we missed the `BindProperty` on the properties in the page model. So we added that

4. After, we had to adjust the validation if statement to work correctly due to a missing negation of the `Email.Contain("@")`
