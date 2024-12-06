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
- .NET Runtime 
- qBittorrent: Make sure qBittorrent is installed on your system and is running.
 Download and install qBittorrent from [](https://choosealicense.com/licenses/mit/) here.
- You will also need to set up environment variables for your qBittorrent username and password. This allows the program to authenticate and interact with your qBittorrent client.
## 1) Download the repository to a folder1
```powershell
mkdir "foldername"
cd "foldername"
git clone "https://github.com/essmann/qbittorrent_rarbg_installer.git"
```
## 2) In Config.JSON insert your qBittorrent username, password and IP address
```json
{
	"IPAddress": "http://yourip:port",
	"Username":  "yourusername",
	"Password":  "yourpassword"
}
```

## 3) Go to qBittorrent -> Tools/Options -> Enable WebUI. Type in your username, password and IP address. Make sure this matches your Config.JSON.

## 4)  Run the Program
	```powershell
	dotnet run "Harry Potter" 
	```
 #### Program will execute, refresh the Terminal after this and you can run the program from anywhere with the alias "torrent" -> torrent "moviename" -options
## Usage
###
 ```yaml
Arguments:
  <name>  The name of the torrent file

Options:
  -tv                      Specify the TV category
  -movies                  Specify the Movies category
  -games                   Specify the Games category
  -music                   Specify the Music category
  --max-pages <max-pages>  The maximum number of pages to search for
  --version                Show version information
  -?, -h, --help           Show help and usage information
```
```yaml


PS C:\coding> torrent "Dexter s01" -tv
# returns sorted sist of Torrents by Seeders:
#Includes resolution, Rip type and codex aswell. 
Sorted List of Torrents by Seeders:
Index | Seeders    | Torrent URL                                        | Details
0     | 368        | dexter-new-blood-s01-complete-720p-amzn-webrip-... | Resolution: 720p, Rip: webrip, Codec: x264
1     | 235        | dexter-new-blood-s01-1080p-amzn-webrip-ddp5-1-x... | Resolution: 1080p, Rip: webrip, Codec: x264
2     | 204        | dexter-new-blood-s01-1080p-webrip-x265-5108253     | Resolution: 1080p, Rip: webrip, Codec: x265
3     | 169        | dexter-new-blood-s01-1080p-bluray-x264-bordure-... | Resolution: 1080p, Rip: N/A, Codec: x264
4     | 146        | dexter-new-blood-s01-webrip-x265-ion265-5107980    | Resolution: N/A, Rip: webrip, Codec: x265
5     | 126        | dexter-new-blood-s01-proper-webrip-x264-ion10-5... | Resolution: N/A, Rip: webrip, Codec: x264
6     | 88         | dexter-new-blood-2021-season-1-s01-1080p-bluray... | Resolution: 1080p, Rip: N/A, Codec: x265
7     | 81         | dexter-new-blood-s01-1080p-bluray-x265-5197098     | Resolution: 1080p, Rip: N/A, Codec: x265
8     | 76         | dexter-season-01-s01-complete-720p-10bit-bluray... | Resolution: 720p, Rip: N/A, Codec: x265
9     | 65         | dexter-new-blood-s01-complete-1080p-amzn-10bit-... | Resolution: 1080p, Rip: N/A, Codec: x265
10    | 60         | dexter-new-blood-s01-720p-amzn-webrip-ddp5-1-x2... | Resolution: 720p, Rip: webrip, Codec: x264
11    | 53         | dexter-s01-1080p-bluray-x264-hdmi-5328514          | Resolution: 1080p, Rip: N/A, Codec: x264
12    | 47         | dexter-season-01-s01-complete-1080p-10bit-blura... | Resolution: 1080p, Rip: N/A, Codec: x265
13    | 39         | dexter-new-blood-s01-720p-bluray-x264-bordure-5... | Resolution: 720p, Rip: N/A, Codec: x264
14    | 37         | dexter-s01-1080p-bluray-x265-5330108               | Resolution: 1080p, Rip: N/A, Codec: x265
Select a Torrent:

--------------------------------------------------------------
```
### After this you will be prompted to select one of these Torrents
```powrshell
Select a Torrent:
```
### Typing in any of the indices will install that torrent file to your qbittorrent.
```powershell
1
Authenticated with qBittorrent.
Magnet link successfully added to qBittorrent.


```
