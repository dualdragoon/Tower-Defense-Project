import sys
import System
sys.path.append(System.IO.Directory.GetCurrentDirectory() + '\Lib')
import clr
clr.AddReference(r'SharpDX.dll')
clr.AddReference(r'SharpDX.Toolkit.Graphics.dll')
print clr.References
#from IronPythonTesting import TestClass
from System.Collections.Generic import List
from SharpDX import Vector2
from SharpDX.Toolkit.Graphics import Texture2D
from Tower_Defense_Project import Tower, Enemy, ProjectileType, Level, Main

class Projectile(object):
	speed = .1
	seconds = 10.0
	damage = 1

	points = List[Vector2]()
	
	def __init__(self, origin, position, target, type, level):
		self.origin = origin
		self.position = position
		self.target = target
		self.type = type
		self.level = level
		
		self.build()
	
	def build(self):
		#obj = TestClass("Flerp")
		#obj.Go();
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
	
	def loadContent(self):
		self.tex = Main.GameContent.Load[Texture2D]("Projectiles/Small Projectile")
	
