# CodeHub
<span class="badge-patreon"><a href="https://www.patreon.com/aalok05" title="Donate to this project using Patreon"><img src="https://img.shields.io/badge/patreon-donate-yellow.svg" alt="Patreon donate button" /></a></span>
[![Twitter URL](https://img.shields.io/badge/tweet-%40devaalok-blue.svg?style=social&style=flat-square)](https://twitter.com/devaalok)

CodeHub is a Universal Windows GitHub client that helps you keep up with the open source world.

<p align="center"><a href="https://www.microsoft.com/store/apps/9nblggh52tbd?ocid=badge"><img src="https://assets.windowsphone.com/85864462-9c82-451e-9355-a3d5f874397a/English_get-it-from-MS_InvariantCulture_Default.png" alt="Get it from Microsoft" width='200' /></a></p>

## Features
* Trending repositories
* News Feed
* View code (with syntax highlighting), issues and comments.
* Create Issues
* Comment on Issues and PRs
* Choose from 9 different syntax highlighting styles
* Search repositories, users, issues and code
* Star, Watch and Fork repositories
* Follow users
* Edit profile

## Screenshots

|               |                   |
|:-------------:| :----------------:|
| ![screenshot](https://raw.githubusercontent.com/aalok05/CodeHub/master/Screenshots/repoView.PNG)  | ![screenshot](https://raw.githubusercontent.com/aalok05/CodeHub/master/Screenshots/trending.PNG) |


## Contributing
Do you want to contribute? Here are our [contribution guidelines](https://github.com/aalok05/CodeHub/blob/master/CONTRIBUTING.md).

## Setting up the project
* [Register](https://github.com/settings/developers) your OAuth application and paste your key and secret in the `app.config` file in the root of the project.

## URI Schemes
You can launch CodeHub and navigate to repositories and user profiles using custom URI schemes

Examples:
- _codehub://repository/aalok05/codehub_
- _codehub://user/aalok05_

## Troubleshooting

### I Can't Find My Organization

CodeHub can see all organizations *if they are granted access*. GitHub, by default, disables [third-party access](https://help.github.com/articles/about-third-party-application-restrictions/) for new organizations. Because of this, CodeHub has no knowledge that those organizations even exist. GitHub keeps that information from the app. There are several ways to correct this. If you own the organization follow [these instructions](https://help.github.com/articles/enabling-third-party-application-restrictions-for-your-organization/). If you do not own the organization you can request access for CodeHub by following [these instructions](https://help.github.com/articles/requesting-organization-approval-for-third-party-applications/).

## Dependencies
I thank the makers of these libraries
* [Octokit](https://github.com/octokit/octokit.net)
* [UICompositionAnimations](https://github.com/Sergio0694/UICompositionAnimations)
* [MVVM Light](https://www.nuget.org/packages/MvvmLightLibs/)
* [UWP Community Toolkit](https://github.com/Microsoft/UWPCommunityToolkit)
* [HTML Agility Pack](https://www.nuget.org/packages/HtmlAgilityPack)
* [LocalNotifications](https://github.com/RavinduL/LocalNotifications)
* [UWP-Animated-SplashScreen](https://github.com/XamlBrewer/UWP-Animated-SplashScreen)
* [QueryString.Net](https://www.github.com/WindowsNotifications/QueryString.Net)

## Gitter chat
* https://gitter.im/CodehubUWP/Bugs
* https://gitter.im/CodehubUWP/FeatureRequests

## Patrons
* Ian Rasmussen

Special thanks to my patrons for supporting me in making UWP apps :)
You can also become a patron [here](https://www.patreon.com/aalok05).
