# arsub
Command line to subscribe and unsubscribe to issues and pull requests based on label combinations

## Install
`dotnet tool install --global arsub`

## Update
`dotnet tool update --global arsub`

## Configure
This is optional to store common parameters and secrets locally. Both --repo and --github-pat config values could be overided by command line arguments of particulars commands.
To get GitHub personal access token visit https://github.com/settings/tokens and create token with at least repo scope. 
This token is never send anywhere else than GitHub API.

`arsub config --repo {owner}/{name} --github-pat {github personal access token}`

## Usage
`arsub subscribe --label area-Serialization --label area-Serialization -since 2010-05-05`
`arsub unsubscribe --label area-Serialization`


