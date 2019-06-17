
Ä
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
y ("A
BattleEntityProperty
key (
value (
ratio ("»
BattleEntity

id (
userid (
camp (
name (	
type (
config (
position (2.Point2D
	direction (2.Point2D)

properties	 (2.BattleEntityProperty"
BattleBeginRequest"#
BattleBeginReturn
result (">
BattleBeginNotify
copy (
list (2.BattleEntity"N
BattleEntityIdleNotify

id (
copy (
position (2.Point2D"|
BattleEntityRunNotify

id (
copy (
position (2.Point2D
velocity (2.Point2D
	movespeed ("ê
BattleEntityAttackNotify

id (
copy (
skill (
duration (
speed (
target (
position (2.Point2D"[
BattleEntityAttackChangeNotify

id (
copy (
duration (
speed ("1
BattleEntityDieNotify

id (
copy ("N
BattleEntityBloodNotify

id (
copy (

hp (
value ("a
BattleEntityPropertyNotify

id (
copy ()

properties (2.BattleEntityProperty"
BattleEndNotify
copy (