# GT4SaveEditor

Save Editor for Gran Turismo 4. Allows progress editing, car editing, hybridding and more.

Decrypts and encrypts the countless layers of save encryptions. Only took 18 years.

## Features & Roadmap

- [x] Decrypting/Encrypting
- [x] Unpacking/Repacking
- [x] Editing Money
- [x] Managing the Used Car Dealership (changing current week, toggling/untoggling available cars)
- [x] Editing Game Settings
- [x] Editing/Hybridding Current Car
- [x] Garage Management (Somewhat, read section below)
- [x] Event Progress Management
- [x] Stats Editing
- [x] Exporting saves to different regions
- [ ] Buttons to quickly set overall progress

## ⚠️ Important Notes

Not all cars can be decrypted correctly, as the game uses shuffling during decryption of cars which involves non IEEE-754 compliant PS2 floating points that cannot be accurately replicated. Nothing can truly be done about it. 

Workaround is to ride the car you want to edit before doing so, as those are not encrypted.

### ⚠️ It also implies that changing Clamping/Rounding settings within PCSX2 may corrupt the save or certain cars in the garage! Be aware! ⚠️ 
