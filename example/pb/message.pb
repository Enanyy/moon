
œ

message.proto".
LoginRequest
name (	
password (	":
LoginReturn
result (
userdata (2	.UserData"$
UserData

id (
name (	"+
LoginGameNotify

ip (	
port ("
LoginGameRequest

id ("!
LoginGameReturn
result ("
Point2D	
x (	
y ("¯
BattleEntityData

hp (
maxhp (
attack (
defense (
	movespeed (
attackspeed (
position (2.Point2D
	direction (2.Point2D"Å
BattleEntity

id (
userid (
camp (
name (	
type (
config (
searchdistance (
attackdistance (
radius	 (
data
 (2.BattleEntityData"
BattleBeginRequest"#
BattleBeginReturn
result (">
BattleBeginNotify
copy (
list (2.BattleEntity"S
BattleEntityIdleNotify

id (
copy (
data (2.BattleEntityData"n
BattleEntityRunNotify

id (
copy (
velocity (2.Point2D
data (2.BattleEntityData"‰
BattleEntityAttackNotify

id (
copy (
skill (
attackspeed (
target (
data (2.BattleEntityData"1
BattleEntityDieNotify

id (
copy ("N
BattleEntityBloodNotify

id (
copy (

hp (
value ("
BattleEndNotify
copy (