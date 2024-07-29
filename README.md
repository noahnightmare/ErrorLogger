# ErrorLogger 
![downloads](https://img.shields.io/github/downloads/noahnightmare/ErrorLogger/total)

## About
A simple plugin used to send all errors within your console to a discord webhook.  
  
The dependency Newtonsoft.Json is needed for this plugin to work! I have provided it within the releases section.  
Place it in EXILED > Plugins > dependencies  
  
Credit to https://github.com/BuildBoy12-SL/ErrorHandler for the original idea.  
This plugin is an updated version of Build's ErrorHandler, as it is outdated and archived.

## Default Config
```yaml
ErrorLogger:
  is_enabled: true
  debug: false
  # Specify which webhook errors should be sent to. 
  webhook_link: ''
```
