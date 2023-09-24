# NetAuthPlayground

Contains various .Net projects around authentication and authorization

## Concepts

Authentication is the process of determining a user's identity. It is responsible for providing a ClaimsPrincipal.

One or more authentication handlers (or "schemes") can be registered to the authentication service.
Each authentication scheme have a name, and a default authentication scheme can be provided to the AddAuthentication method.

Based on its options and the HTTP request context, an AuthenticationHandler will try to authenticate the user. If successful it create an AuthenticationTicket

Then authentication middlewere is registered using the UseAuthentication method. authentication middleware will use the previously registered authentication schemes in AddAuthentication.

Important methods/actions available:

- **ChallengeAsync**: invoked by Authorization when an unauthenticated request comes in to an endpoint that requires authentication. An example of challenge is to redirect the user to a login page.
- **AuthenticateAsync**: construct a ClaimsPrincipal representing a user's identity based on request context. For example a cookie authentication handler constructs a ClaimsPrincipal from an incoming cookie.
- **SignInAsync**: takes a ClaimsPrincipal (typially created in AuthenticateAsync) and persist it. For example the SignInAsync of the cookie authentication handler will persist the principal to an encrypted cookie.
- **ForbidAsync**: invoked by Authorization when an authenticated user tries to access resource he's not allowed.
- **SignOutAsync** Reverse of SignInAsync, it will instruct the middleware to delete any persisted data (e.g: cookie)
