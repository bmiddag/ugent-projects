lucas:
	cmpl	$1, %eax
	jg	.L2
	movl	$2, %eax
	ret
.L2:
	movl	%eax, 4(%esp)
	subl	$8, %esp
	cmpl	$3, %eax
	jle	.L3
	subl	$1, %eax
	call	lucas
	movl	%eax, 4(%esp)
	movl	12(%esp), %eax
	cmpl	$4, %eax
	je	.L4
	subl	$2, %eax
	call	lucas
	shl	%eax
	addl	4(%esp), %eax
	addl	$8, %esp
	ret
.L3:
	leal	-2(,%eax,4), %eax
	addl	$8, %esp
	ret
.L4:
	movl	$12, %eax
	addl	4(%esp), %eax
	addl	$8, %esp
	ret