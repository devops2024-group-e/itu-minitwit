# What is being changed?

> **Date:** 09/02/24
>
> **Author** Andreas Tietgen

The task is to convert the code from python to C# as best as possible. The challenge is to the same logic and behavior as in the python code.

## What did we do?
1. Created a `Login.cshtml` that contains the same logic as in the python version by adding pieces of the page one at a time.

2. Added an in memory user (let us call it a dummy user) as we do not have the database connected for now and we just want to see if the ui logic actually works when a user tries to login

3. Adding a session that should store the `userid` as it does in the python application. We do this by adding the following lines as settings in the `Program.cs`:
    ```C#
    builder.Services.AddDistributedMemoryCache();

    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(5);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

    ...

    app.UseSession();
    ```
4. Now we can get and set session data by using `this.HttpContext.Session.SetString()` and  `this.HttpContext.Session.GetString()` in a `PageModel` class

5. Add if statement that if the login is successful then it should store userid and redirect to `/Timeline` (Timeline should take care if it should show a public or user timeline)
