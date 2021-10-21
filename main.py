from dataclasses import dataclass, field
from datetime import datetime, timedelta
from kivy.app import App
from kivy.uix.label import Label
from kivy.uix.stacklayout import StackLayout
from kivy.uix.scrollview import ScrollView
from kivy.uix.button import Button
from kivy.properties import StringProperty, partial, ObjectProperty
from kivy.uix.screenmanager import ScreenManager, Screen
from kivy.lang import Builder
from collections import OrderedDict
from kivy.core.window import Window


@dataclass
class EmployeeInfo:
    """Class for keeping track of relevant employee information"""
    hire_date: datetime
    FOH: bool
    BOH: bool
    leadership: bool
    birth: datetime
    last_eval: datetime = field(init=False)
    day_45: bool = False
    day_90: bool = False

    def __post_init__(self):
        self.last_eval = self.hire_date

    def __str__(self):
        return "Hire_Date: " + str(self.hire_date.date()) + " FOH: " + str(self.FOH) + " BOH: " + str(self.BOH) + \
               " Leadership: " + str(self.leadership) + " 45 Day Eval: " + str(self.get_45().date()) + " 90 Day Eval: " \
               + str(self.get_90().date()) + " Next Eval: " + str(self.get_next_eval().date())

    def get_45(self) -> datetime:
        """Returns the date time for a 45 day eval"""
        time_change = timedelta(days=45)
        return self.hire_date + time_change

    def get_90(self) -> datetime:
        """Returns the date time for a 90 day eval"""
        time_change = timedelta(days=90)
        return self.hire_date + time_change

    def get_next_eval(self) -> datetime:
        """Returns a datetime for the next eval, should occur every 6 months"""
        time_change = timedelta(weeks=24)
        return self.last_eval + time_change


@dataclass()
class PerformanceEntry:
    """An individual entry for the performance of an employee"""
    leader_name: str
    category: str
    subcategory: str
    description: str
    date: datetime


class PerformanceTracker:
    """Tracks performance for an employee"""
    def __init__(self):
        self.performance = []

    def create_entry(self, leader_name, category, subcategory, description, date):
        """Creates a new entry in the performance of an employee"""
        new = PerformanceEntry(leader_name, category, subcategory, description, date)
        self.performance.append(new)

    def sort_date(self):
        """Sorts all of the performance tracker by the dates of the entries"""
        self.performance.sort(key=lambda x: x.date)


class Employee:
    """Class for defining an employee"""
    employee_info: EmployeeInfo
    name: str
    pin: str
    performance: PerformanceTracker

    def __init__(self, name: str, pin: str, employee_info):
        self.name = name
        self.pin = pin
        self.performance = PerformanceTracker()
        self.employee_info = employee_info

    def __str__(self):
        return "Name: " + self.name + " Pin: " + self.pin

    def complete_eval(self, date_completed: datetime, day_45=False, day_90=False):
        """Method for completing evaluations"""
        if day_45:
            self.employee_info.day_45 = True
        elif day_90:
            self.employee_info.day_90 = True
        else:
            self.employee_info.last_eval = date_completed


class EmployeeRegistry:
    """Class for containing a registry of all employees"""

    def __init__(self, **kwargs):
        self.employee_registry = {}

    def add_employee(self, employee: Employee):
        self.employee_registry[employee.name] = employee

    def remove_employee(self, employee_name: str):
        del self.employee_registry[employee_name]

    def get_alerts(self) -> dict:
        """This function returns a dictionary of all the alerts for employee's incomplete evaluations"""
        day_45 = []
        day_90 = []
        month_6 = []
        for employee in self.employee_registry:
            if not employee.employee_info.day_45 and datetime.today() > employee.employee_info.get_45():
                day_45.append(employee)
            if not employee.employee_info.day_90 and datetime.today() > employee.employee_info.get_90():
                day_90.append(employee)
            if datetime.today() > employee.employee_info.get_next_eval():
                month_6.append(employee)
        return {"45 Day Evals": day_45, "90 Day Evals": day_90, "6 Month Evals": month_6}


my_birthday = datetime.strptime('Mar 10 2000', '%b %d %Y')
my_hiredate = datetime.strptime('Sep 14 2020', '%b %d %Y')


class NewEmployee(Screen):
    temp_hire_date = None
    temp_name = StringProperty("")
    temp_pin = "0"
    temp_FOH = False
    temp_BOH = False
    temp_Leadership = False
    temp_birth = None

    def create_employee(self):
        App.get_running_app().EmployeeReg.add_employee(Employee(self.temp_name, self.temp_pin,
                                                                EmployeeInfo(self.temp_hire_date, self.temp_FOH,
                                                                             self.temp_BOH, self.temp_Leadership,
                                                                             self.temp_birth)))
        print("New Employee Created")

    def employee_name(self, text_input):
        self.temp_name = text_input.text

    def pin(self, text_input):
        self.temp_pin = text_input.text

    def foh(self, text_input):
        self.temp_FOH = bool(text_input.text)

    def boh(self, text_input):
        self.temp_BOH = bool(text_input.text)

    def leadership(self, text_input):
        self.temp_Leadership = bool(text_input.text)

    def hire_date(self, text_input):
        self.temp_hire_date = datetime.strptime(text_input.text, '%d/%m/%Y')

    def birth(self, text_input):
        self.temp_birth = datetime.strptime(text_input.text, '%d/%m/%Y')


class EmployeeRecords(Screen):

    def __init__(self, **kwargs):
        super(EmployeeRecords, self).__init__(**kwargs)
        self.name = "EmployeeRecords"
        self.layout = StackLayout()
        self.employee_layout = StackLayout()
        self.performance_layout = ScrollView(size_hint=(None, 1), size=(Window.width-Window.width/5, 50000))

    def add_employees(self):
        """get_updated_employees().employee_registry should be an Employee_Registry class, this function will
        convert them into buttons"""
        reg = get_updated_employees().employee_registry
        reg = OrderedDict(sorted(reg.items()))  # This makes it so it is always sorted alphabetically
        for employee in reg:
            b = Button(text=employee, size_hint=(.2, .2))
            b.bind(on_release=partial(self.display_employee, reg[b.text]))  # partial creates a separate function
            # that will be referenced later. If we used lambda instead that would cause b.text to refer to the most
            # recently referenced object b, which will always be the last employee in reg.
            self.layout.add_widget(b)
        self.add_widget(self.layout)

    def display_employee(self, *args):
        """Displays employee information on the same screen that previously contained the names of employees"""
        employee = args[0]
        name = Label(text=employee.name, font_size=32, size_hint=(.2, .2))
        pin = Label(text=employee.pin, font_size=32, size_hint=(.2, .2))
        foh = Label(text="Employee is FOH: " + str(employee.employee_info.FOH), font_size=32, size_hint=(.4, .2))
        # if you make the size_hint too small for the text then it will overlap with other labels, hence why the size
        # for foh is larger.
        self.employee_layout.add_widget(pin)
        self.employee_layout.add_widget(name)
        self.employee_layout.add_widget(foh)
        self.employee_layout.add_widget(Button(text="Main Menu", on_release=lambda
            x: self.main_menu_er(), size_hint=(.2, .2)))

        performance = StackLayout(size_hint=(1, None), height=Window.height+Window.height/5, width=Window.width,
                                  orientation="tb-lr")
        for entry in employee.performance.performance:
            print("I did a thing")
            performance.add_widget(Label(text="Leader Name: " + entry.leader_name + "\nCategory: " +
                                                           entry.category + "\nSub Category: " + entry.subcategory +
                                                           "\nDate: " + str(entry.date) + "\nDescription: " +
                                                           entry.description, size_hint=(.2, .2)))
            performance.add_widget(Label(text="123", size_hint=(.2, .2)))
        self.performance_layout.add_widget(performance)
        self.remove_widget(self.layout)
        self.add_widget(self.employee_layout)
        self.add_widget(self.performance_layout)

    def main_menu_er(self):
        """Navigates to main menu and resets everything in EmployeeRecords"""
        self.remove_widget(self.performance_layout)
        self.remove_widget(self.employee_layout)
        self.employee_layout = StackLayout()
        self.performance_layout = ScrollView(size_hint=(None, 1), size=(Window.width-Window.width/5, Window.height-Window.height/5))
        self.layout = StackLayout()
        App.get_running_app().root.transition.direction = "down"
        App.get_running_app().root.current = "Menu"


def get_updated_employees():
    return App.get_running_app().EmployeeReg


class Menu(Screen):
    pass


class WindowManager(ScreenManager):
    pass


kv = Builder.load_file('demo.kv')


class DemoApp(App):
    EmployeeReg = EmployeeRegistry()
    Wes = Employee("Wesley", '733427', EmployeeInfo(datetime.strptime('Mar 10 2000', '%b %d %Y'), True, False, True,
                                                    datetime.strptime('Sep 13 2020', '%b %d %Y')))
    Jeena = Employee("Jeena", '10000', EmployeeInfo(datetime.strptime('Mar 10 1996', '%b %d %Y'), False, False, True,
                                                    datetime.strptime('Sep 06 2019', '%b %d %Y')))
    Wilson = Employee("Wilson", '1', EmployeeInfo(datetime.strptime('Mar 10 1990', '%b %d %Y'), True, True, True,
                                                  datetime.strptime('Sep 06 2019', '%b %d %Y')))
    Wes.performance.create_entry('Wesley', 'Tardy', 'No Call', 'They were late and did not call in',
                             datetime.strptime('Sep 13 2020', '%b %d %Y'))
    Wes.performance.create_entry('Wesley', 'Tardy', 'No Call', 'Test asdlkjfalskjdflkdajflkasdjflkasdjflkjdfa;lsdjf',
                             datetime.strptime('Sep 13 2020', '%b %d %Y'))
    Jeena.performance.create_entry('Wesley', 'Tardy', 'No Call', 'hey 123',
                             datetime.strptime('Sep 13 2020', '%b %d %Y'))
    Wilson.performance.create_entry('Wesley', 'Tardy', 'No Call', 'fff456',
                             datetime.strptime('Sep 13 2020', '%b %d %Y'))
    EmployeeReg.add_employee(Wes)
    EmployeeReg.add_employee(Jeena)
    EmployeeReg.add_employee(Wilson)

    def build(self):
        return kv


if __name__ == '__main__':
    DemoApp().run()
