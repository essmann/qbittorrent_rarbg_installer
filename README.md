## CLI Torrent Scraper & Installer 
This is a command-line interface (CLI) tool designed for installing torrents from RaRBG directly to your qBittorrent client. It provides a streamlined process to fetch and manage torrents based on movie names or other search criteria.

## Features
- Search for torrents based on user-defined keywords (e.g., movie titles).
- Instantly fetch the top torrent results from popular torrent sites.
- Seamlessly download torrents using qBittorrent.
## Getting Started
Prerequisites
Before running the CLI tool, you need the following installed:

- .NET SDK (version 8.0 or higher)
- qBittorrent: Make sure qBittorrent is installed on your system and is running.
 Download and install qBittorrent from [](https://choosealicense.com/licenses/mit/) here.
- You will also need to set up environment variables for your qBittorrent username and password. This allows the program to authenticate and interact with your qBittorrent client.
### Setting up environment variables on Windows (Powershell)
```powershell
setx qbtusername "insert username here";
setx qbtpassword "insert password here";
```
## Usage

```powershell
dotnet run <"name"> <-category>

```
### Categories are: -tv, -movies, -all
```powershell
dotnet run "Harry Potter and the Goblet of Fire" -movies # Not case-sensitive. e.g., Harry Potter == HARRY potter 
# returns sorted sist of Torrents by Seeders:
[0] | harry-potter-and-the-goblet-of-fire-2005-1080p-bluray-dd-5-1-x265-edge2020-6076816 | 178 | 2024
--------------------------------------------------------------
[1] | harry-potter-and-the-goblet-of-fire-2005-1080p-bluray-x264-ac3-soup-6253038 | 130 | 2024
--------------------------------------------------------------
[2] | harry-potter-and-the-goblet-of-fire-2005-1080p-bluray-h264-aac-r4rbg-tgx-5618909 | 98 | 2023
--------------------------------------------------------------
[3] | harry-potter-and-the-goblet-of-fire-2005-2160p-bluray-x265-ddp-dts-kingdom-6245924 | 90 | 2024
--------------------------------------------------------------
[4] | harry-potter-and-the-goblet-of-fire-2005-eng-720p-hd-webrip-2-06gib-aac-x264-portalgoods-6000504 | 62 | 2024

--------------------------------------------------------------
```
### After this you will be prompted to select one of these Torrents
```powrshell
Select a Torrent:
```
### Typing in any of the indices will install that torrent file to your qbittorrent.
```powershell
1
Selected torrent: https://rargb.to/torrent/harry-potter-and-the-goblet-of-fire-2005-1080p-bluray-x264-ac3-soup-6253038.html
Authenticated with qBittorrent.
Magnet link successfully added to qBittorrent.


```
