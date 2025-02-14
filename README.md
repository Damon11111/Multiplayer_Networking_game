# cs426_Asgn2

Team Memebers: Semih Kesler, Yuqing Wu, Junaid Ali

Group Number: 11

Title: Courier vs Robber

Game Description: 1 Courier must deliever all of their 13 packages to each mailbox in a row without getting caught to win
1 Robber must chase and stop the courier 

Player Interaction Pattern: Player vs Player

Objective: Capture, Outwit - Courier tries to avoid and block robber from steal and deliver all their packages to mailboxes to win and Robber must attempt to
chase, stall, and steal from the courier to win

Serious Objective: Show how data transfers can be vulnerable to outside interference or theft and why protection against them is important

Procedures:<br>
  Who - 1 Courier and 1 Robber<br>
  What - Delivery player can deliver to target mailboxes when close and robber can steal when close to opposing player both can throw rocks to stun opposing player<br>
  Where - City<br>
  When - Couriers must start at spawn for each delivery<br>
  How - Interacting with target mailbox or player in close proximety<br>
  Rules:  Players interact with eachother by throw rocks, robbers can steal when close to courier, couriers can deliver when close to target mailbox<br>

Resourses:<br>
    Player Controller:<br>
      https://assetstore.unity.com/packages/3d/characters/modular-first-person-controller-189884 <br> 
    City:<br>
      https://assetstore.unity.com/packages/3d/environments/urban/toon-city-pack-234785<br>
    Script References:<br>
      https://youtu.be/yhlyoQ2F-NM?t=139 - Singleton Instance (Manager Scripts)<br>
      https://docs-multiplayer.unity3d.com/netcode/current/basics/networkvariable/ NetworkVariable (All)<br>
      https://docs.unity3d.com/Packages/com.unity.netcode.gameobjects@1.7/api/Unity.Netcode.RuntimeTests.SpawnTest.OnNetworkSpawn.html NetworkSpawn (All)<br>
      https://docs-multiplayer.unity3d.com/netcode/current/advanced-topics/message-system/serverrpc/ ServerRpc (All)<br>
      https://docs-multiplayer.unity3d.com/netcode/current/advanced-topics/message-system/clientrpc/ ClientRpc (All)<br>
      
Non-plain-vanilla procedure/rule: Players can throw rocks at eachother to stun them temporarily

Test Question for Serious Objective: Why is it important to protect package deliveries. What can happen if you don't?
Expected Correct Answer for Serious Objective: Data Streams(Packages) are vulnerable to interference and stealing and should be protected to prevent it

