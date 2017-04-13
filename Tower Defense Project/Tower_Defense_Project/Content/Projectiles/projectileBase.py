import sys
import System
sys.path.append(System.IO.Directory.GetCurrentDirectory() + '\Lib')
import clr
clr.AddReference(r'Duality.dll')
clr.AddReference(r'SharpDX.dll')
clr.AddReference(r'SharpDX.Toolkit.Game.dll')
clr.AddReference(r'SharpDX.Toolkit.Graphics.dll')
from System.Collections.Generic import List
from Duality import ErrorHandler
from SharpDX import Vector2, Color
from SharpDX.Toolkit import GameTime
from SharpDX.Toolkit.Graphics import Texture2D, SpriteBatch
from Tower_Defense_Project import Tower, Enemy, Level, Main

idNum = -1

class ProjectileBase(object):
	speed = .1
	seconds = 10.0
	damage = 1
	
	stageIndex = 0
	stagePos = 0
	
	points = List[Vector2]()
	
	def __init__(self, origin, position, target, level):
		self.origin = origin
		self.position = position
		self.target = target
		self.level = level
		
		self.Build()
		self.LoadContent()
	
	def Build(self):
		self.points.Clear()
		self.points.Add(self.position)
		self.points.Add(self.target.position)
		
		self.lengths = [None] * (self.points.Count - 1)
		self.directions = [None] * (self.points.Count - 1)
		
		for i in range(0, self.points.Count - 1):
			self.directions[i] = self.points[i + 1] - self.points[i]
			self.lengths[i] = self.directions[i].Length()
			self.directions[i].Normalize()
		
	
	def LoadContent(self):
		self.tex = Main.GameContent.Load[Texture2D]("Projectiles/Small Projectile")
	
	def OnHit(self):
		self.target.Health -= self.damage
		self.level.projectiles.Remove(self)
	
	def Update(self, gameTime):
		self.Build()
		
		try:
			self.UpdateMovement()
		except Exception, Argument:
			ErrorHandler.RecordError(2, 102, "Help", str(Argument))
		
	
	def UpdateMovement(self):
		if (self.stageIndex != self.points.Count - 1):
			self.stagePos = self.stagePos + (self.speed * self.seconds)
			while (self.stagePos > self.lengths[self.stageIndex]):
				self.stagePos = self.stagePos - self.lengths[self.stageIndex]
				self.stageIndex = self.stageIndex + 1
				if (self.stageIndex == self.points.Count - 1):
					self.position = self.points[self.stageIndex]
					return
				
			self.position = self.points[self.stageIndex] + Vector2(self.directions[self.stageIndex].X * self.stagePos, self.directions[self.stageIndex].Y * self.stagePos)
			
		if (self.stageIndex == 1):
			self.OnHit()
		if (not self.origin.range.Contains(self.position)):
			self.level.projectiles.Remove(self)
		
	
	def Draw(self, spriteBatch):
		try:
			self.level.DrawProjectile(spriteBatch, self.tex, self.position, Color.White)
		except Exception, Argument:
			ErrorHandler.RecordError(2, 102, "Help", str(Argument))
		
	
