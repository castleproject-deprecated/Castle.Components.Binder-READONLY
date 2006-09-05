## Copyright 2004-2006 Castle Project - http:##www.castleproject.org/
## 
## Licensed under the Apache License, Version 2.0 (the "License");
## you may not use this file except in compliance with the License.
## You may obtain a copy of the License at
## 
##     http:##www.apache.org/licenses/LICENSE-2.0
## 
## Unless required by applicable law or agreed to in writing, software
## distributed under the License is distributed on an "AS IS" BASIS,
## WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
## See the License for the specific language governing permissions and
## limitations under the License.

require 'rubygems'
require 'watir'
require 'test/unit'
require 'test/unit/ui/console/testrunner'

include Watir

require 'accountpermission'
require 'productlicense'
require 'account'
require 'category'

# Test cases for MonoRail ActiveRecord support

## -- Account Permission --
##
## Creates three Account Permissions
##
class AccountPermissionTestCase < Test::Unit::TestCase

	def setup
		@ie = IE.new()
		@ie.set_fast_speed
	end
	
	def teardown
		@ie.close
	end

	def test_crud()
		id = AccountPermission.create(@ie, 'account permission 1')
		
		AccountPermission.edit(@ie, id, 'new name')
		
		AccountPermission.delete(@ie, id)
	end

end

class ProductLicenseTestCase < Test::Unit::TestCase

	def setup
		@ie = IE.new()
		@ie.set_fast_speed
	end
	
	def teardown
		@ie.close
	end

	def test_crud()
		id = ProductLicense.create(@ie, '1/2/2006', '2/2/2007')
		
		ProductLicense.edit(@ie, id, '1/3/2006', '2/4/2007')
		
		ProductLicense.delete(@ie, id)
	end
	
end

class AccountTestCase < Test::Unit::TestCase

	def setup
		@ie = IE.new()
		@ie.set_fast_speed
	end
	
	def teardown
		@ie.close
	end

	def test_crud()
		ap1 = AccountPermission.create(@ie, 'account permission 1')
		ap2 = AccountPermission.create(@ie, 'account permission 2')
		pl1 = ProductLicense.create(@ie, '1/2/2006', '2/2/2007')
		
		# Uses the standard approach to send multiple values (indexed node)
		id = Account.create(@ie, 'account name', 'hammett@gmail.com', '123', '123', pl1, ap1, ap2)
		
		Account.edit(@ie, id, 'new account name', 'hammett@apache.org', 'xpto', 'xpto', pl1, ap2)
		
		Account.delete(@ie, id)
		
		# Uses a different approach to send multiple values (leafnode with array)
		id = Account.create2(@ie, 'account name', 'hammett@gmail.com', '123', '123', pl1, ap1, ap2)
		
		Account.delete(@ie, id)
	end
	
end

class CategoryTestCase < Test::Unit::TestCase

	def setup
		@ie = IE.new()
		#@ie.set_fast_speed
	end
	
	def teardown
		#@ie.close
	end

	def test_crud()
		id = Category.create(@ie, 'name')
		
		Category.edit(@ie, id, 'new name')
		
		Category.delete(@ie, id)
	end
	
end


class CastleTests
	def self.suite
		suite = Test::Unit::TestSuite.new
#		suite << AccountPermissionTestCase.suite
#		suite << ProductLicenseTestCase.suite
#		suite << AccountTestCase.suite
		suite << CategoryTestCase.suite
		return suite
	end
end

Test::Unit::UI::Console::TestRunner.run(CastleTests)


