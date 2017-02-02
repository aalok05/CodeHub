# CodeHub
<span class="badge-patreon"><a href="https://www.patreon.com/aalok05" title="Donate to this project using Patreon"><img src="https://img.shields.io/badge/patreon-donate-yellow.svg" alt="Patreon donate button" /></a></span>
<span class="badge-paypal"><a href="https://www.paypal.me/aalok05" title="Donate to this project using Paypal"><img src="https://img.shields.io/badge/paypal-donate-yellow.svg" alt="PayPal donate button" /></a></span>

CodeHub is a Universal Windows GitHub client that helps you keep up with the open source world.

Get it on the Windows store [here](https://www.microsoft.com/en-us/store/p/codehub-a-client-for-github/9nblggh52tbd#).

![codehub-logo](/CodeHub/Assets/Images/appLogoPurple.png?raw=true)

#Features
* See Trending repositories
* View code, issues and comments. 
* Search repositories, users, issues and code
* Star repositories
* Follow people

#Contributions
Pull requests are welcome in the `dev` branch!

#Setting up the project 
* [Register](https://github.com/settings/developers) your OAuth application and paste your key and secret in the `app.config` file in the root of the project.
* Don't forget to add a reference to [UICompositionAnimations](https://github.com/Sergio0694/UICompositionAnimations)

#Things to be done
CodeHub aims to do all those things that the GitHub Desktop app doesn't do. Our goal is to make an app for UWP which lets you keep up with the open source world on the go.
I think these features are needed in CodeHub:
* Find out a better way to get Trending repositories (web page scraping is done as of now)
* Fork repos
* Trending developers
* Syntax highlighting for code
* Search code, issues (Implemented)

#Dependencies
I thank the makers of these libraries
* [Octokit](https://github.com/octokit/octokit.net)
* [UICompositionAnimations](https://github.com/Sergio0694/UICompositionAnimations)
* [MVVM Light](https://www.nuget.org/packages/MvvmLightLibs/)
* [UWP Community Toolkit](https://github.com/Microsoft/UWPCommunityToolkit)
* [MarkdownSharp.UWP](https://www.nuget.org/packages/MarkdownSharp.UWP/)
* [HTML Agility Pack](https://www.nuget.org/packages/HtmlAgilityPack)
* [JetBrains ReSharper Annotations](https://www.nuget.org/packages/JetBrains.Annotations)
* [Lumia Imaging SDK UWP](https://www.nuget.org/packages/LumiaImagingSDK.UWP/)

#Gitter chat
* https://gitter.im/CodehubUWP/Bugs
* https://gitter.im/CodehubUWP/FeatureRequests
