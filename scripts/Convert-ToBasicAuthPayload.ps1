<#
.SYNOPSIS
    This script generates a Basic Authorization header.

.DESCRIPTION
    This script generates a Basic Authorization header using the provided username and password.

.PARAMETER name
    The username to be included in the Basic Authorization header.

.PARAMETER password
    The password to be included in the Basic Authorization header.

.EXAMPLE
    .\Convert-ToBasicAuthPayload.ps1 -name "JohnDoe" -password "myPassword"
#>

param(
    [string]$name = "JohnDoe",
    [string]$password = "password"
)

$credentials = "$($name):$password";
"Basic $([Convert]::ToBase64String([Text.Encoding]::UTF8.GetBytes($credentials)))"