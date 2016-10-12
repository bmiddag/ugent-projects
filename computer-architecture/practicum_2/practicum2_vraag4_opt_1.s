lucas:
	cmpl	$1, %eax
	jg	.L2
	movl	$2, %eax
	ret
.L2:
	subl	$20, %esp
	#vanaf hier
	movl	24(%esp), %eax
	subl	$1, %eax
	movl	%eax, (%esp)
	call	lucas
	movl	%eax, 8(%esp)
	movl	24(%esp), %eax
	subl	$2, %eax
	movl	%eax, (%esp)
	call	lucas
	addl	%eax, %eax
	addl	8(%esp), %eax
	#tot hier
	addl	$20, %esp
	ret