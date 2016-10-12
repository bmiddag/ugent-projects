lucas:
	cmpl	$1, %eax
	jg	.L2
	movl	$2, %eax
	ret
.L2:
	movl	%eax, 4(%esp)
	subl	$20, %esp
	#vanaf hier
	subl	$1, %eax
	call	lucas
	movl	%eax, 8(%esp)
	movl	24(%esp), %eax
	subl	$2, %eax
	call	lucas
	#movl	%eax, %ebx
	#addl	%eax, %eax
	shl	%eax
	addl	8(%esp), %eax
	#tot hier
	addl	$20, %esp
	ret