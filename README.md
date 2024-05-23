# calendar-finder
The `CalendarFinder` is an app to find all calendars of a user and some user-data in an easy way without making any `http` requests in ms-graph.

## Usage
1. Create a file (like mydata.json) put the `Azure Ad App` Registration and your user-identifiers as shown in the example.
2. Start the app with the input file (step 1) as parameter.
3. The result file (like mydata.result.json) will be opened in `Visual Studio Code` automatically.

### Example
#### Input Data As Json
```json
{
  "MsGraphClients": [
    {
      "TenantId": "adzure_ad_app_tenant_guid",
      "ClientId": "adzure_ad_app_client_guid",
      "Thumbprint": "certificate_thumbprint",
      "Scopes": null
    }
  ],
  "UserIdentifier": [
    "my_guid_user_identifier",
    "my_user_principle_name_identifier",
    "my_user_mail_identifier"
  ]
}
```
#### Start Process (CMD)
```cmd
> NRG.CalendarFinder.exe -f mydata.json
```
#### Result Data As Json
```json
[
  {
    "Input": {
      "UserIdentifier": "my_input_user_identifier"
    },
    "User": {
      "DisplayName": "output_display_name",
      "PrincipalName": "output_user_principla_name",
      "Mail": "output_user_mail",
      "UserId": "output_user_guid",
      "CalendarId": "only_or_default_calendar_id"
    },
    "Calendars": [
      {
        "Id": "calendar_id",
        "Name": "calendar_name",
        "Owner": "owner_mail",
        "IsDefault": true
      }
    ]
  }
  { 
    ...
  } 
]
```

## Needs
- [Appregistration (Azure Ad App)](https://learn.microsoft.com/en-us/entra/identity-platform/quickstart-register-app)
- Necessary Permissions on the App (Azure Ad App)

## Nice To Know
### Need Help In Console To Use The App?
Type `--help` to see the help text and the required and optional arguments.
### Supress File Opening After Finish
To not open the file in `Visual Studio Code` after finish searching, use the command with the argument `-e false` (e is for editor).
### How Are The Users Found?
By default `ms-graph` allows to find users directly by providing the user-id or user-principle-name.
That's how this order came about:
1. user-id / user-principle-name
2. user-mail
3. user-mail-nickname\*
4. user-principle-nickname\*  
\**the mail-nickname is the part left of the @ in the mail-address (nickname@provider.com -> nickname)*

## Used Libraries
- [Command Line Parser](https://github.com/commandlineparser/commandline)
- [Ms Graph](https://github.com/microsoftgraph/msgraph-sdk-dotnet)
- [Azure Identity](https://github.com/Azure/azure-sdk-for-net)
- [Microsoft.Extensions.Hosting](https://github.com/dotnet/runtime)