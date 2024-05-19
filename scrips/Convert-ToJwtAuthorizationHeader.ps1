<#
.SYNOPSIS
    This script generates a JWT authorization header.

.DESCRIPTION
    This script generates a JWT authorization header using the provided role and secret. 
    If no secret is provided, it fetches the secret from the appsettings.development.json file.

.PARAMETER role
    The role to be included in the JWT payload.

.PARAMETER secret
    The secret used to sign the JWT. If not provided, the script will fetch it from the appsettings.development.json file.

.EXAMPLE
    .\Convert-ToJwtAuthorizationHeader.ps1 -role "admin" -secret "mySecret"
#>

param(
    [string]$role,
    [string]$secret = ""
)

Function Get-Base64([string]$inputValue) {
    $bytes = [Text.Encoding]::UTF8.GetBytes($inputValue)
    $result = [System.Convert]::ToBase64String($bytes, [Base64FormattingOptions]::None)
    return $result
}

$header = [ordered]@{
    alg = "HS256"
    typ = "JWT"
}

$payload = [ordered]@{
    iat = [DateTimeOffset]::Now.AddHours(-1).ToUnixTimeSeconds()
    exp = [DateTimeOffset]::Now.AddHours(1).ToUnixTimeSeconds()
    sub = "admin@todoapp.com"
    "day" = "Monday"
}

if ($role) {
    $payload.Add("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", $role)
}

$headerJson = $header | ConvertTo-Json -Compress
$payloadJson = $payload | ConvertTo-Json -Compress

$headerEnc = (Get-Base64 -input $headerJson).TrimEnd('=')
$payloadEnc = (Get-Base64 -input $payloadJson).TrimEnd('=')

$headerPayload = $headerEnc + "." + $payloadEnc

if ($secret -eq "") {
    $path = Split-Path -Parent $PSCommandPath
    $json = Get-Content "$path\..\src\TodoApp.API\appsettings.development.json" -Raw | ConvertFrom-Json
    $secret = $json.Security.TokenSecret
}

$hmacsha = New-Object System.Security.Cryptography.HMACSHA256
$hmacsha.Key = [Text.Encoding]::UTF8.GetBytes($secret)
$signature = $hmacsha.ComputeHash([Text.Encoding]::UTF8.GetBytes($headerPayload))
$signature = [Convert]::ToBase64String($signature)
$signature = $signature.TrimEnd('=')

$headerPayload + '.' + $signature