import sys
import System
sys.path.append(System.IO.Directory.GetCurrentDirectory() + '\Lib')
import clr
clr.AddReference(r'SharpDX.dll')
clr.AddReference(r'SharpDX.Toolkit.Game.dll')
clr.AddReference(r'SharpDX.Toolkit.Graphics.dll')
print clr.References
from System.Collections.Generic import List
from SharpDX import Vector2, Color
from SharpDX.Toolkit import GameTime
from SharpDX.Toolkit.Graphics import Texture2D, SpriteBatch
from Tower_Defense_Project import Tower, Enemy, ProjectileType, Level, Main

idNum = 0

class Projectile(object):
	speed = .1
	seconds = 10.0
	damage = 1

	stageIndex = 0

	points = List[Vector2]()
	
	def __init__(self, origin, position, target, type, level):
		self.origin = origin
		self.position = position
		self.target = target
		self.type = type
		self.level = level
		
		self.Build()
	
	def Build(self):
		self.points.Clear()
		self.points.Add(self.position)
		self.points.Add(self.target.position)

		self.lengths = []
		self.directions = []

		for i in range(0, self.points.Count - 1):
			self.directions[i] = self.points[i + 1] - self.points[i]
			self.lengths[i] = self.directions[i].Length()
			self.directions[i].Normalize()
		print "Hello"
	
	def LoadContent(self):
		self.tex = Main.GameContent.Load[Texture2D]("Projectiles/Small Projectile")
	
	def OnHit(self):
		self.target.Health -= self.damage
		self.level.projectiles.Remove(self)
	
	def Update(self, gameTime):
		self.build()

		if (self.stageIndex != self.points.Count - 1):
			self.stagePos += self.speed * self.seconds
			while (self.stagePos > self.lengths[self.stageIndex]):
				self.stagePos -= self.lengths[self.stageIndex]
				self.stageIndex = self.stageIndex + 1
				if (self.stageIndex == self.points.Count - 1):
					self.position = self.points[self.stageIndex]
					return
			self.position = self.points[self.stageIndex] + self.directions[self.stageIndex] * self.stagePos
		if (self.stageIndex == 1):
			self.onHit()
		if (not self.origin.range.Contains(self.position)):
			self.level.projectiles.Remove(self)
		
	
	def Draw(self, spriteBatch):
		try:
			spriteBatch.Draw(self.tex, self.position, Color.White)
		except:
			print ""
		
	
