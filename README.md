# RPG-Game-Backend

[![CodeFactor](https://www.codefactor.io/repository/github/nemeslaszlo/rpg-game-backend/badge)](https://www.codefactor.io/repository/github/nemeslaszlo/rpg-game-backend)

Simple RPG (role-playing game) game backend, which has a very similar concept like Shakes and Fidget, in text based role-playing game style.

- .NET 5
- AutoMapper
- NLog
- Swagger
- Entity Framework Core
- Identity and Authentication
- Security with JSON Web Token

#### Endpoints for the Backend

More about the endpoints like json in Swagger

(One character has only one weapon, there is no limit to the skills)

| Entity    | Type   | URL                  | Description                                                                                                                        | Success                        | Authorize             |
| --------- | ------ | -------------------- | ---------------------------------------------------------------------------------------------------------------------------------- | ------------------------------ | --------------------- |
| User      | POST   | /api/auth/register   | User registration.                                                                                                                 | 200 OK                         | No                    |
|           | POST   | /api/auth/login      | User login as a Player.                                                                                                            | 200 Ok response with the token | No                    |
| Character | GET    | /api/characters      | Get the user's (player) all characters. Admin gets all character.                                                                  | 200 OK                         | Yes (Player or Admin) |
|           | GET    | /api/characters/{id} | Get the user's (player) by id. Admin can get every character.                                                                      | 200 OK                         | Yes (Player or Admin) |
|           | POST   | /api/characters      | Create a new character.                                                                                                            | 201 Created                    | Yes (Player)          |
|           | PUT    | /api/characters/{id} | Update a character. (Players can update his/her characters only.)                                                                  | 204 NoContent                  | Yes (Player or Admin) |
|           | DELETE | /api/characters/{id} | Delete a character. (Players can delete his/her characters only.)                                                                  | 204 NoContent                  | Yes (Player or Admin) |
| Weapon    | POST   | /api/weapon          | Add a weapon to one of your character. (Players can use this with their characters only.)                                          | 200 OK                         | Yes (Player)          |
|           | PUT    | /api/weapon          | Change one of your character's weapon. (Players can use this with their characters only.)                                          | 200 OK                         | Yes (Player)          |
|           | DELETE | /api/weapon          | Delete/Drop one of your character's weapon. (Players can use this with their characters only.)                                     | 200 OK                         | Yes (Player)          |
| Skill     | POST   | /api/characterskill  | Add a new skill to one of your character. (Players can use this with their characters only.)                                       | 200 OK                         | Yes (Player)          |
|           | DELETE | /api/characterskill  | Delete a character skill. (Players can use this with their characters only.)                                                       | 200 OK                         | Yes (Player)          |
| Fight     | POST   | /api/fight/weapon    | Attack the opponent with weapon.                                                                                                   | 200 OK                         | Yes (Player)          |
|           | POST   | /api/fight/skill     | Attack the opponent with skill.                                                                                                    | 200 OK                         | Yes (Player)          |
|           | POST   | /api/fight           | Fight with the joined characters until the first death. (The executor wins the round)                                              | 200 OK                         | Yes (Player)          |
|           | POST   | /api/fight/deathmach | Last one surviving character wins. (One of your character able to kill one of your other character like other players characters.) | 200 OK                         | Yes (Player)          |
|           | GET    | /api/fight/highscore | Character leaderboard.                                                                                                             | 200 OK                         | No                    |
