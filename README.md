## AutoDefenseBattleMobileGame

AutoDefenseBattleMobileGame is a vertical mobile defense game prototype built in Unity.
The game focuses on scalable turret systems, upgrade progression, and dynamic UI architecture.

## 🚀 Core Features

- Fortress-based defense gameplay
- Homing missile turret combat system
- Scalable turret upgrade system (Damage, Cooldown, future stats)
- Dynamic turret detail UI (supports different turret stat layouts)
- Persistent player progression using JSON save data
- Scroll-based turret selection UI
- Modular architecture for future turret expansion


## ⚙ Technical Highlights

- Modular Turret Upgrade Architecture
Upgrade logic is centralized using helper systems, allowing new turrets to be added with minimal UI or logic changes.
- Data Separation Design
----------------------------------------
PlayerData → Save file structure       | 
PlayerProfile → Runtime gameplay state |
----------------------------------------
This allows clean persistence and runtime stat management.


## 👨‍💻 Author Note

Developed as part of a personal portfolio project focusing on gameplay systems and scalable architecture design. Aspiring to become a professional game developer.

## 🛠 Project Status

This project is actively being expanded with additional turret types, upgrade systems, and gameplay polish.