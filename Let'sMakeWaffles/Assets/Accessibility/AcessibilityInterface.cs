using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface AcessibilityInterface  {
	string getWaitText();
	void changeState(bool state, bool isUrgent = false);
	void moveForward();
	void moveBackward();
	void select();
	void unselect();
	void infoText();
	void revertOption();
	void toggleNavigation(bool state);
	int returnListLength();

	void ToogleGreenBoxState(bool state);
	
}
