; installeer de schedulerhandler op de timeronderbreking en zet deze onderbreking aan (Opgave 6 van de voorbereiding)
	push	schedulerhandler
	push	32
	call	install_handler
	cli				; disable interrupts
	in	al, 0x21
	and	al, 0feh		; mask 11111110
	out	0x21, al
	sti				; enable interrupts