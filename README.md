# SignalR Progress Bar
SignalR Progress Bar Asp.Net Core 2.2 (Asp.net Identity &amp; Stripe)

Hello World!

My name is Randy and this is my first Github open source contribution. I was a technical architect for Accenture and later Avanade in my first life (many years ago). This past year I have decided to get back into tech. It has taken me a bit to catch back up on all the great tech out there. After 12 months, I feel like I am still just scratching the surface! Enough about me... You probably want to know about the Signalr Progress Bar.

I spent the past 3 days learning SignalR. My first instinct is to always search github and stackoverflow for examples. It's a bad habit that I am trying to break! What I should do is start with the Microsoft docs because they really are fantastic. Unfortunately, I read the Docs last... which probably would have saved me 2 days of experiments.

If you want to learn about SignalR, start here: https://docs.microsoft.com/en-us/aspnet/core/signalr/introduction?view=aspnetcore-2.2

Having said that, the tutorials and example code did not show how to communicate with a single user in a business layer ie Stripe Check Out Progress Bar. To compound matters, I found several examples of signalR progress bars, but none of them were current which caused some pain when trying to use the latest tech with old code. In the end, I spent the time to fully digested the docs and was able to create exactly what I wanted. My only criticism with the Docs is with the Javascript Client API information. It's not complete and there are a couple of errors in the docs.

SignalR is awesome be they way... After successfully completing my project, I thought I would share what I learned.

The goal:

1. Create a Progress Bar using SignalR. (cause spinners are so last year!)
2. Report progress using Asp.net Identity Users.
3. Progress Bar to be updated from a business layer.
4. Client to resond to Server actions through SignalR methods. ie Stripe Checkout Button.
5. Handle and report exceptions using SignalR.

This example uses a Stripe Elements Credit Card element. In order for this example to work, you will need a Stripe publishable key (free from stripe).

Pre-Reqs:

1. Stripe publishable key. Create a free Stripe account @ stripe.com to get one.
Add a Stripe section to your managed secrets in this format: 
"Stripe": { 
"StripePublishableKey": "your key goes here", 
"StipeSecretKey": "secret key here"
}
2. update-database (This is a standard MVC template with individual user accounts. All you have to do is apply the initial migration.

I struggled with finding a way to communicate with a single signed in user. In my first experiments, I grabbed the connectionId from the Hub context and OnConnectedAsync I returned the connectionId to the client through a parameter on the "connected" method. This seemed clunky to me and also created other issues because a user can have many connectionId's (Desktop, iPad, Phone etc). So communicating using a connectionId was not ideal. Learning about SignalR's IUserIdProvider was an "aha" moment for me! Great work Microsoft!
