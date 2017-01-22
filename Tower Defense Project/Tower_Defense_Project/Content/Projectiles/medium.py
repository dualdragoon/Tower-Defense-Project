import sys
import System
sys.path.append(System.IO.Directory.GetCurrentDirectory() + '\Lib')
sys.path.append(System.IO.Directory.GetCurrentDirectory() + '\Content\Projectiles')
import clr
clr.AddReference(r'Duality.dll')
clr.AddReference(r'SharpDX.dll')
clr.AddReference(r'SharpDX.Toolkit.Game.dll')
clr.AddReference(r'SharpDX.Toolkit.Graphics.dll')
import projectileBase
from projectileBase import ProjectileBase
from System.Collections.Generic import List
from Duality import ErrorHandler
from SharpDX import Vector2, Color
from SharpDX.Toolkit import GameTime
from SharpDX.Toolkit.Graphics import Texture2D, SpriteBatch
from Tower_Defense_Project import Tower, Enemy, Level, Main

idNum = 1

class Projectile(ProjectileBase):
	speed = .1
	seconds = .6
	damage = 2

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

	def LoadContent(self):
		self.tex = Main.GameContent.Load[Texture2D]("Projectiles/Medium Projectile")

		def OnHit(self):
			self.target.Health -= self.damage
			self.level.testProjectiles.Remove(self)

		def Update(self, gameTime):
			self.Build()

			self.UpdateMovement()

		def Draw(self, spriteBatch):
			self.level.DrawProjectile(spriteBatch, self.tex, self.position, Color.White)


